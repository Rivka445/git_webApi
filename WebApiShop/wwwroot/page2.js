const titleName = document.querySelector(".titleName")
const firstName = (JSON.parse(sessionStorage.getItem("currentUser"))).firstName
titleName.textContent = `ברוכה הבאה ${firstName} מייד נצלול פנימה`

const extrctDataFromInput = () => {
    const id = Number(JSON.parse(sessionStorage.getItem("currentUser")).id)
    const userName = document.querySelector("#userName").value
    const firstName = document.querySelector("#firstName").value
    const lastName = document.querySelector("#lastName").value
    const password = document.querySelector("#password").value
    return { id,userName, firstName, lastName, password }
}

async function upDate() {
    const currenrtUser = extrctDataFromInput()
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
            sessionStorage.setItem("currentUser", JSON.stringify(currenrtUser))
            alert("המשתמש עודכן בהצלחה")
        }
    }
    catch (e) {alert(e) }
}

async function checkPassword() {
    const bar = document.querySelector(".bar")
    let password = document.querySelector("#password").value
    if (password === "") {
        password = "1"
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
            throw new Error(`HTTP error! status ${response.status}`);
        }        
        else {
            const a = await response.json() 
            bar.innerHTML = ""
            bar.style.display = "flex"
            for (let i = 0; i < a; i++) {
                const step = document.createElement("div")
                step.className = "stage"
                bar.appendChild(step)
            }
        }
    }
    catch (e) {
        bar.innerHTML = ""
        alert(e)
        return 0
    }
}
