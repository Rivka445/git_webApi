const extrctDataFromInputValid = () => {
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
    let usersArrayExist = JSON.parse(sessionStorage.getItem("Users"))
    if (usersArrayExist===null) {
        sessionStorage.setItem("Users", JSON.stringify([]))
        usersArrayExist = JSON.parse(sessionStorage.getItem("Users"))
    }
    const isExist = !!usersArrayExist.find(user => user.userName ===userName && user.password ===password)
    if (isExist === true) {
        alert("שם משתמש וסיסמא תפוסים בחר שם משתמש או סיסמא שונים")
        return
    }
    return { userName, firstName, lastName, password }
}

const createObjUser = (upDateValues) => {
    const id = 1
    const userName = upDateValues.userName
    const firstName = upDateValues.firstName
    const lastName = upDateValues.lastName
    const password = upDateValues.password
    return { id, userName, firstName, lastName, password }
}

const updateStorage = (fullUser) => {
    sessionStorage.setItem("currentUser", JSON.stringify(fullUser))
}

async function postResponse() {
    const dateValues = extrctDataFromInputValid()
    if (dateValues === "") {
        return
    }
    const newUser = createObjUser(dateValues)
    try {
        const response = await fetch(
            "https://localhost:44362/api/Users",
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newUser)
            }
        )
    if (!response.ok) {
        throw new Error(`HTTP error! status ${response.status}`);
        }
    else {
        alert("המשתמש נרשם בהצלחה")
        const newUserFull = await response.json()
        const usersArray = JSON.parse(sessionStorage.getItem("Users"))
        usersArray.push(newUserFull)
        sessionStorage.setItem("Users", JSON.stringify(usersArray))
        }
    }
     catch (e) { alert(e) }
}

async function logIn() {
    const userName = document.querySelector("#username").value
    const password = document.querySelector("#pasword").value
    if (!userName ||!password) {
        alert("לפחות אחד מהנתונים חסרים")
        return 
    }
    const existUser = { userName, password }
    try {
        const response = await fetch(
            "https://localhost:44362/api/Users/login",
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(existUser)
            }
        )
        if (!response.ok) {
            alert("שם משתמש או סיסמא שגויים")
            throw new Error(`HTTP error! status ${response.status}`);
        }
        else {
            const fullUser = await response.json()
            updateStorage(fullUser)
            window.location.href = "page2.html"
        }
    }
    catch (e) { alert(e) }
}


