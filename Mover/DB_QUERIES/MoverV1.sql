
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
-- For UserDetail Table
ALTER TABLE UserDetail DROP CONSTRAINT fk_AspUserId;
ALTER TABLE UserDetail
    ADD CONSTRAINT fk_AspUserId 
    FOREIGN KEY (AspUserId) 
    REFERENCES AspNetUsers(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For Products Table
ALTER TABLE ProductImages DROP CONSTRAINT FK_ProductImages_Products;
ALTER TABLE ProductImages
    ADD CONSTRAINT FK_ProductImages_Products 
    FOREIGN KEY (ProductID) 
    REFERENCES Products(ProductID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For Address Table
ALTER TABLE Address DROP CONSTRAINT FK_Address_UserDetail;
ALTER TABLE Address
    ADD CONSTRAINT FK_Address_UserDetail 
    FOREIGN KEY (UserID) 
    REFERENCES UserDetail(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For Orders Table
ALTER TABLE Orders DROP CONSTRAINT FK_Orders_UserDetail;
ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_UserDetail 
    FOREIGN KEY (UserID) 
    REFERENCES UserDetail(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For OrderItems Table
ALTER TABLE OrderItems DROP CONSTRAINT FK_OrderItems_Orders;
ALTER TABLE OrderItems
    ADD CONSTRAINT FK_OrderItems_Orders 
    FOREIGN KEY (OrderID) 
    REFERENCES Orders(OrderID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

ALTER TABLE OrderItems DROP CONSTRAINT FK_OrderItems_Products;
ALTER TABLE OrderItems
    ADD CONSTRAINT FK_OrderItems_Products 
    FOREIGN KEY (ProductID) 
    REFERENCES Products(ProductID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For ShoppingCart Table
ALTER TABLE ShoppingCart DROP CONSTRAINT FK_ShoppingCart_UserDetail;
ALTER TABLE ShoppingCart
    ADD CONSTRAINT FK_ShoppingCart_UserDetail 
    FOREIGN KEY (UserID) 
    REFERENCES UserDetail(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

ALTER TABLE ShoppingCart DROP CONSTRAINT FK_ShoppingCart_Products;
ALTER TABLE ShoppingCart
    ADD CONSTRAINT FK_ShoppingCart_Products 
    FOREIGN KEY (ProductID) 
    REFERENCES Products(ProductID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For ProductReviews Table
ALTER TABLE ProductReviews DROP CONSTRAINT FK_ProductReviews_Products;
ALTER TABLE ProductReviews
    ADD CONSTRAINT FK_ProductReviews_Products 
    FOREIGN KEY (ProductID) 
    REFERENCES Products(ProductID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

ALTER TABLE ProductReviews DROP CONSTRAINT FK_ProductReviews_UserDetail;
ALTER TABLE ProductReviews
    ADD CONSTRAINT FK_ProductReviews_UserDetail 
    FOREIGN KEY (UserID) 
    REFERENCES UserDetail(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;

-- For Inventory Table
ALTER TABLE Inventory DROP CONSTRAINT FK_Inventory_Products;
ALTER TABLE Inventory
    ADD CONSTRAINT FK_Inventory_Products 
    FOREIGN KEY (ProductID) 
    REFERENCES Products(ProductID) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE;



    -- Drop the unique constraint
ALTER TABLE [dbo].[ShoppingCart] DROP CONSTRAINT [UserProduct];

-- Drop the foreign keys
ALTER TABLE [dbo].[ShoppingCart] DROP CONSTRAINT [FK_ShoppingCart_Products];
ALTER TABLE [dbo].[ShoppingCart] DROP CONSTRAINT [FK_ShoppingCart_UserDetail];

ALTER TABLE [dbo].[ShoppingCart] ALTER COLUMN [UserID] INT NOT NULL;
ALTER TABLE [dbo].[ShoppingCart] ALTER COLUMN [ProductID] INT NOT NULL;
ALTER TABLE [dbo].[ShoppingCart] ALTER COLUMN [Quantity] INT NOT NULL;
ALTER TABLE [dbo].[ShoppingCart] ALTER COLUMN [AddedAt] DATETIME NOT NULL;

-- Recreate the foreign key for Products
ALTER TABLE [dbo].[ShoppingCart] ADD CONSTRAINT [FK_ShoppingCart_Products]
FOREIGN KEY ([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON UPDATE CASCADE
ON DELETE CASCADE;

-- Recreate the foreign key for UserDetail
ALTER TABLE [dbo].[ShoppingCart] ADD CONSTRAINT [FK_ShoppingCart_UserDetail]
FOREIGN KEY ([UserID])
REFERENCES [dbo].[UserDetail] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE;
