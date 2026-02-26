# 👗 מערכת השכרת שמלות - צד שרת (REST API)

מערכת Backend לניהול השכרת שמלות, ממומשת כ-**REST API** מודרני באמצעות **ASP.NET 9** ו-**C#**.  
המערכת תוכננה עם דגש על ביצועים גבוהים, סקילביליות (Scalability) והפרדה מלאה בין שכבות הלוגיקה והנתונים.

---

## 🏗 ארכיטקטורה ומבנה הפרויקט

הפרויקט בנוי ב-**ארכיטקטורת 3 שכבות**, המאפשרת תחזוקה קלה, בדיקות איכותיות וניתוק תלויות:

1. **שכבת Application (Web API)**  
   - ניהול Controllers והגדרות Routing  
   - מימוש Middlewares לטיפול בבקשות HTTP וניהול שגיאות  
   - הגדרת **Dependency Injection** מרכזית  

2. **שכבת Services**  
   - שכבת הלוגיקה העסקית  
   - מתווכת בין Controllers ל-Repositories  
   - ביצוע אימותים ועיבוד נתונים  
   - מבוצעת בצורה **אסינכרונית** לשחרור משאבי שרת  

3. **שכבת Repositories**  
   - גישה לנתונים באמצעות **Repository Pattern**  
   - שימוש ב-**Entity Framework Core** בגישת **Database First**  
   - פעולות CRUD מבוצעות באופן **אסינכרוני** לשיפור ביצועים וסקילביליות  

---

## 🛠 מאפיינים טכנולוגיים ודגשים

### ⚡ ביצועים וסקילביליות
- **תכנות אסינכרוני:** שימוש ב-`async/await` בכל השכבות לשחרור Threads וליכולת Scale גבוהה  
- **ניתוק תלויות:** שימוש ב-**Dependency Injection (DI)** ליצירת קוד מודולרי וגמיש  

### 🔄 ניהול נתונים ומיפוי
- **DTOs (Data Transfer Objects):** שכבת DTO למניעת תלויות מעגליות והפרדה בין מסד הנתונים ל-API  
- **C# Records:** DTOs מיוצגים כ-`records` להבטחת אובייקטים Immutable והעברת נתונים יעילה  
- **AutoMapper:** מיפוי אוטומטי בין Entities ל-DTOs לשמירה על קוד נקי  

### 📊 ניטור, לוגים וניהול שגיאות
- **NLog:** רישום פעולות המערכת ושגיאות  
- **Error Handling Middleware:** טיפול אחיד בשגיאות והפניה ללוגים  
- **Auditing:** כל התעבורה והדירוגים נשמרים בטבלת `Rating` לצורך ניתוח ומעקב  
- **Configuration:** קונפיגורציות נשמרות בקבצי `appsettings.json` מחוץ לקוד  

---

## 🧪 בדיקות

- **Unit Tests:** בדיקות יחידות מבודדות של השירותים  
- **Integration Tests:** בדיקות אינטגרציה לוודא סינכרון בין כל השכבות ומסד הנתונים  

---

## 📂 מבנה תיקיות

```text
├── DressRental.API          # Controllers, Middlewares, AppSettings
├── DressRental.Services     # Business Logic, Interfaces, AutoMapper Profiles, DTOs
├── DressRental.Repositories # DB Context, Entities (EF), Repository Implementations
└── DressRental.Tests        # Unit & Integration Tests
