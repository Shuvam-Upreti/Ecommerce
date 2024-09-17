
---====//Initial migration for AspNet tables//====---

--Add-Migration AddUserDetailTable -Context MoverContext
--Update-Database -Context MoverContext



---======//Scaffolding Query put Default project as Mover.Core compulsary //==========---
---Scaffold-DbContext "Server=localhost;Port=5432;Database=Mover;User ID=postgres;Password=pass@word1;" -OutputDir "Entities" Npgsql.EntityFrameworkCore.PostgreSQL -force -context "MoverContext" -NoOnConfiguring

---======//Database Query//==========---

--Shuvam 9/17/2024
CREATE TABLE IF NOT EXISTS public."UserDetail"
(
	"Id" serial PRIMARY KEY,
	"FullName" varchar(100) NOT NULL,
	"AspUserId" varchar(100) NOT NULL,
	"DateOfJoin" timestamp NOT NULL,
	"Department" varchar(50),
	CONSTRAINT fk_AspUserId FOREIGN KEY ("AspUserId") REFERENCES public."AspNetUsers"("Id")
);
