
---====//Initial migration for AspNet tables//====---

--Add-Migration AddUserDetailTable -Context MoverContext
--Update-Database -Context MoverContext



---======//Scaffolding Query put Default project as Mover.Core compulsary //==========---
---Scaffold-DbContext "Server=localhost;Port=5432;Database=Mover;User ID=postgres;Password=pass@word1;" -OutputDir "Entities" Npgsql.EntityFrameworkCore.PostgreSQL -force -context "MoverContext" -NoOnConfiguring
---Scaffold-DbContext "Server=(localdb)\MSSQLLocalDB;Database=EcommerceDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir "Entities" -force -context "MoverContext" -NoOnConfiguring


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

CREATE TABLE Category
(
	Id int identity(1,1) PRIMARY KEY,
	[Name] varchar(100) NOT NULL unique,
	CreatedOn datetime2 NOT NULL DEFAULT GETDATE()
);

-- Products Table
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    OriginalPrice DECIMAL(10, 2) NOT NULL,
    DiscountedPrice DECIMAL(10, 2),
	DiscountPercentage DECIMAL(5, 2),
    CategoryID INT,
    FOREIGN KEY (CategoryID) REFERENCES Category(Id)
);

-- Product Images Table
CREATE TABLE ProductImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT,
    ImageURL NVARCHAR(255),
    IsMainImage BIT DEFAULT 0,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Addresses Table (For Saved User Addresses)
CREATE TABLE Address (
    AddressID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    AddressLine NVARCHAR(255),
    City NVARCHAR(255),
    State NVARCHAR(255),
    ZipCode NVARCHAR(20),
    FOREIGN KEY (UserID) REFERENCES UserDetail(Id)
);

-- Orders Table (Merged Address Information)
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    OrderStatus NVARCHAR(50) DEFAULT 'Pending',
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending',
    TotalAmount DECIMAL(10, 2),
    ShippingAddressLine NVARCHAR(255),
    ShippingCity NVARCHAR(255),
    ShippingState NVARCHAR(255),
    ShippingZipCode NVARCHAR(20),
    FOREIGN KEY (UserID) REFERENCES UserDetail(Id)
);

-- Order Items Table
CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT,
    ProductID INT,
    Quantity INT,
    PriceAtPurchase DECIMAL(10, 2),
    DiscountAtPurchase DECIMAL(10, 2),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Shopping Cart Table
CREATE TABLE ShoppingCart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    ProductID INT,
    Quantity INT,
    AddedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES UserDetail(Id),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UserProduct UNIQUE (UserID, ProductID)
);

-- Product Reviews Table
CREATE TABLE ProductReviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT,
    UserID INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES UserDetail(Id)  
);

-- Inventory Table
CREATE TABLE Inventory (
    InventoryID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT,
    QuantityInStock INT DEFAULT 0,
    LastStockUpdate DATETIME,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
