
const extrctDataFromInputUser = () => {
    const userName = document.querySelector("#userName").value
    const firstName = document.querySelector("#firstName").value
    const lastName = document.querySelector("#lastName").value
    const password = document.querySelector("#password").value
    const id=0
    return {id, userName, firstName, lastName, password }
}

const extrctDataFromInputLogIn = () => {
    const userName = document.querySelector("#username").value
    const password = document.querySelector("#pasword").value
    const id = 1,firstName= "aaa", lastName ="aaa"
    return { id, userName, firstName , lastName , password }
}

async function registIn() {
    const newUser = extrctDataFromInputUser()
    try {
        const response = await fetch (
            "https://localhost:44362/api/Users",{
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
        }
    }
    catch (e) { alert(e) }
}

async function logIn() {
    const existUser = extrctDataFromInputLogIn()
    try {
        const response = await fetch(
            "https://localhost:44362/api/Users/login",{
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(existUser)
            }
        )
        if (!response.ok) {
            throw new Error(`HTTP error! status ${response.status}`);
        }
        else {
            const fullUser = await response.json()
            sessionStorage.setItem("currentUser", JSON.stringify(fullUser))
            window.location.href = "page2.html"
        }
    }
    catch (e) { alert(e) }
}
async function checkPassword() {
    const bar = document.querySelector(".bar") 
    let password = document.querySelector("#password").value
    if (password==="") {
        password="1"
    }
    const userPassword = { password }
    try {
        const response = await fetch(
                "https://localhost:44362/api/UsersPassword", {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(userPassword)
            }
        )
        if (!response.ok) {
            throw new Error(`HTTP error! status ${response.status}`)
        }
        else {
            const score = await response.json() 
            bar.innerHTML = ""
            bar.style.display="flex"
            for (let i = 0; i < score; i++) {  
                const step = document.createElement("div")
                step.className ="stage"
                bar.appendChild(step)
            }
        }
    }
    catch (e) {
        bar.innerHTML = ""
        alert(e)
    }
}


