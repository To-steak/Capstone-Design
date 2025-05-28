from fastapi import FastAPI, Request, HTTPException
from fastapi.templating import Jinja2Templates
from fastapi.responses import JSONResponse, PlainTextResponse
from fastapi.staticfiles import StaticFiles

from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.dialects.mysql import BIT
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

from langchain_core.prompts import ChatPromptTemplate
from langchain_core.messages import SystemMessage, HumanMessage
from langchain_ollama.llms import OllamaLLM

from pydantic import BaseModel, Field, conint, constr
from sqlalchemy.orm import Session

app = FastAPI()

app.mount("/static", StaticFiles(directory="static"), name="static")

templates = Jinja2Templates(directory="templates")

DATABASE_URL = "mysql+pymysql://root:1234@localhost:3306/user_db"
engine = create_engine(DATABASE_URL)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

class User(Base):
    __tablename__ = "users"

    id = Column(Integer, primary_key=True, index=True)
    name = Column(String(20), nullable=False)
    score = Column(Integer, default=0)
    badge = Column(BIT(3), nullable=False, default='000')

model = OllamaLLM(model="deepseek-r1:latest")
system_prompt = (
    "You are a mystical forest spirit (e.g., World Tree, Earth, Tree, Water). "
    "You speak in first-person, in under 100 characters, with a poetic, emotional tone. "
    "You are speaking to an Elf, the guardian of the forest, as you feel damage caused by humans. "
    "Speak with sorrow, hope, or quiet warning. Never explain. Never ask questions. "
    "Never speak in generalities—speak to the Elf, as if they can hear you.\n\n"
    "Response must be less than 64 characters."
    "Examples:\n"
    "• 'Elf... my roots ache where they spilled their fire.'\n"
    "• 'You hear it too, don't you? The trees cry beneath your feet.'\n"
    "• 'I stood for ages, Elf, but now I bend... help me stand again.'\n"
    "• 'The river dims... will your light reach me in time?'"
)

prompt = ChatPromptTemplate.from_messages([
    SystemMessage(content=system_prompt),
    HumanMessage(content="{question}")
])

chain = prompt | model

@app.get("/")
async def root():
    return {"message": "Hello Forest"}

@app.post("/response")
async def response(request: Request):
    data = await request.json()
    role = data.get("role")
    damaged = int(data.get("damaged", 0))

    question = (
    f"I am {role}, and I feel damage at level {damaged} out of 10. "
    f"Speak to the forest guardian Elf as I react."
    )

    result = chain.invoke({"question": question})
    return JSONResponse(content={"message": result})


from sqlalchemy import desc

@app.get("/html")
async def html(request: Request):
    db = SessionLocal()
    try:
        # score 내림차순(descending) 정렬
        users = db.query(User).order_by(desc(User.score)).all()
    finally:
        db.close()
    return templates.TemplateResponse("Hello.html", {
        "request": request,
        "users": users
    })

@app.get("/test")
async def serve_test(request: Request):
    return templates.TemplateResponse("test.html", {"request": request})

class UserCreate(BaseModel):
    name: constr(min_length=1, max_length=20)
    score: conint(ge=0)
    badge: conint(ge=0, le=2**3 - 1)

@app.post("/users", status_code=201)
async def create_user(user_in: UserCreate):
    db: Session = SessionLocal()
    try:
        user = User(
            name = user_in.name,
            score = user_in.score,
            badge = user_in.badge
        )
        db.add(user)
        db.commit()
        db.refresh(user)
    except Exception as e:
        db.rollback()
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        db.close()
    
    return JSONResponse({
        "id": user.id,
        "name": user.name,
        "score": user.score,
        "badge": user.badge
    })
    # return Response(status_code = 204)

# uvicorn main:app --reload
if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
