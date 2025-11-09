const titleName = document.querySelector(".titleName")
const firstName = (JSON.parse(sessionStorage.getItem("currentUser"))).firstName
titleName.textContent = `ברוכה הבאה ${firstName} מייד נצלול פנימה`

const extrctDataFromInputValid =() => {
    const userName = document.querySelector("#userName").value
    if (userName.indexOf("@") < 1 && userName) {
        alert("השם חייב להכיל @ באמצע")
        return ""
    }
    const firstName = document.querySelector("#firstName").value
    const lastName = document.querySelector("#lastName").value
    const password = document.querySelector("#password").value
    if (password.length < 4 && password) {
        alert("אורך הסיסמא קצר מידי")
        return ""
    }
    if (!userName || !firstName || !lastName || !password) {
        alert("לפחות אחד מהנתונים חסרים")
        return ""
    } 
    return { userName, firstName, lastName, password }
}

const createObjUser = (upDateValues) => {
    const id = Number(JSON.parse(sessionStorage.getItem("currentUser")).id)
    const userName = upDateValues.userName
    const firstName = upDateValues.firstName
    const lastName = upDateValues.lastName
    const password = upDateValues.password
    return { id, userName, firstName, lastName, password }
}

const updateStorage = (upDateValues) => {
    sessionStorage.setItem("currentUser", JSON.stringify(upDateValues))
}

async function upDate() {
    const upDateValues = extrctDataFromInputValid()
    if (upDateValues === "") {
        return
    }
    const currenrtUser = createObjUser(upDateValues)
    try {
        const response = await fetch(
            `https://localhost:44362/api/Users/${currenrtUser.id}`,
            {
                method: `PUT`,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(currenrtUser)
            }
        )
        if (!response.ok) {
            throw new Error(`HTTP error! status ${response.status}`);
        }
        else {
            updateStorage(currenrtUser)
            alert("המשתמש עודכן בהצלחה")
        }
    }
    catch (e) {
        alert(e)
    }
}
