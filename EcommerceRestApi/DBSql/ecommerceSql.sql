
GO
CREATE DATABASE "OnlineStore";
GO

CREATE TABLE Customers (
    Id INT IDENTITY PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    Points INT DEFAULT 0,
    Nip VARCHAR(50),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Customers_AspNetUsers FOREIGN KEY (UserId)
        REFERENCES AspNetUsers(Id)
);
CREATE INDEX IX_Customers_UserId ON Customers(UserId);


CREATE TABLE Countries (
    Id INT IDENTITY PRIMARY KEY,
    CountryName VARCHAR(50) NOT NULL,
    CountryCode VARCHAR(5) NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);

CREATE UNIQUE INDEX UQ_Countries_Code ON Countries(CountryCode);
CREATE UNIQUE INDEX UQ_Countries_Name ON Countries(CountryName);


CREATE TABLE Addresses (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    CountryId INT NULL,
    Street VARCHAR(100),
    HouseNumber VARCHAR(20),
    FlatNumber VARCHAR(20),
    City VARCHAR(50),
    State VARCHAR(50),
    PostalCode VARCHAR(10),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Addresses_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id),

    CONSTRAINT FK_Addresses_Countries FOREIGN KEY (CountryId)
        REFERENCES Countries(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Addresses_CustomerId ON Addresses(CustomerId);
CREATE INDEX IX_Addresses_CountryId ON Addresses(CountryId);


CREATE TABLE Categories (
    Id INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(MAX) NOT NULL,
    Name NVARCHAR(20) NOT NULL,
    About NVARCHAR(500) NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);


CREATE TABLE Subcategories (
    Id INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    About NVARCHAR(500) NOT NULL,
    CategoryId INT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Subcategories_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id)
);

CREATE INDEX IX_Subcategories_CategoryId ON Subcategories(CategoryId);

CREATE TABLE Products (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Brand NVARCHAR(50) NOT NULL,
    Code NVARCHAR(30) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    OldPrice DECIMAL(18,2),
    About NVARCHAR(500),
    LongAbout NVARCHAR(2000) NOT NULL,
    RatingSum FLOAT,
    RatingVotes INT,
    Photo NVARCHAR(255),
    OtherPhotos NVARCHAR(1000),
    Stock INT NOT NULL,
    SubcategoryId INT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Products_Subcategories FOREIGN KEY (SubcategoryId)
        REFERENCES Subcategories(Id)
);

CREATE INDEX IX_Products_SubcategoryId ON Products(SubcategoryId);


CREATE TABLE ProductCategories (
    ProductId INT NOT NULL,
    CategoryId INT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    PRIMARY KEY (ProductId, CategoryId),

    CONSTRAINT FK_PC_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id),

    CONSTRAINT FK_PC_Category FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id)
);

CREATE INDEX IX_ProductCategories_CategoryId ON ProductCategories(CategoryId);


CREATE TABLE Orders (
    Id INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount MONEY NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending',
    IsPaid BIT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id)
);

CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);


CREATE TABLE OrderItems (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice MONEY NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),

    CONSTRAINT FK_OrderItems_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id)
);


CREATE TABLE Coupons (
    Id INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    DiscountAmount DECIMAL(18,2),
    DiscountPercentage DECIMAL(5,2),
    MinOrderAmount DECIMAL(18,2),
    MaxDiscountAmount DECIMAL(18,2),
    StartDate DATETIME2,
    EndDate DATETIME2,
    UsageLimit INT DEFAULT 1,
    UsedCount INT DEFAULT 0,
    PerUserLimit INT,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);

CREATE UNIQUE INDEX IX_Coupons_Code ON Coupons(Code);


CREATE TABLE OrderCoupons (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    CouponId INT NOT NULL,
    DiscountApplied DECIMAL(18,2),
    AppliedAt DATETIME2 DEFAULT GETDATE(),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_OrderCoupons_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE CASCADE,

    CONSTRAINT FK_OrderCoupons_Coupon FOREIGN KEY (CouponId)
        REFERENCES Coupons(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_OrderCoupons_OrderCoupon
ON OrderCoupons(OrderId, CouponId);


CREATE TABLE PaymentMethods (
    Id INT IDENTITY PRIMARY KEY,
    PaymentType VARCHAR(50) NOT NULL,
    Details VARCHAR(100),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);


CREATE TABLE Payments (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NULL,
    PaymentMethodId INT NULL,
    Amount MONEY NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Payments_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),

    CONSTRAINT FK_Payments_Method FOREIGN KEY (PaymentMethodId)
        REFERENCES PaymentMethods(Id)
);

CREATE INDEX IX_Payments_OrderId ON Payments(OrderId);
CREATE INDEX IX_Payments_PaymentMethodId ON Payments(PaymentMethodId);


CREATE TABLE Reviews (
    Id INT IDENTITY PRIMARY KEY,
    ProductId INT NOT NULL,
    CustomerId INT NOT NULL,
    Rating INT NOT NULL,
    ReviewText TEXT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Reviews_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id),

    CONSTRAINT FK_Reviews_Customer FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id)
);

CREATE INDEX IX_Reviews_ProductId ON Reviews(ProductId);
CREATE INDEX IX_Reviews_CustomerId ON Reviews(CustomerId);


CREATE TABLE Wishlists (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Name NVARCHAR(100) DEFAULT 'Default Wishlist',
    IsDefault BIT DEFAULT 0,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME2 DEFAULT GETDATE(),
    DateUpdated DATETIME2,
    DateDeleted DATETIME2,

    CONSTRAINT FK_Wishlist_Customer FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Wishlists_CustomerId ON Wishlists(CustomerId);


CREATE TABLE WishlistItems (
    Id INT IDENTITY PRIMARY KEY,
    WishlistId INT NOT NULL,
    ProductId INT NOT NULL,
    Notes NVARCHAR(500),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_WishlistItems_Wishlist FOREIGN KEY (WishlistId)
        REFERENCES Wishlists(Id) ON DELETE CASCADE,

    CONSTRAINT FK_WishlistItems_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_WishlistItems_Unique
ON WishlistItems(WishlistId, ProductId);


CREATE TABLE DeliveryMethods (
    Id INT IDENTITY PRIMARY KEY,
    MethodName VARCHAR(50) NOT NULL,
    Cost MONEY NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);


CREATE TABLE DeliveryMethodOrders (
    DeliveryMethodId INT NOT NULL,
    OrderId INT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT PK_DeliveryMethodOrders PRIMARY KEY (DeliveryMethodId, OrderId),

    CONSTRAINT FK_DeliveryMethodOrders_Method FOREIGN KEY (DeliveryMethodId)
        REFERENCES DeliveryMethods(Id),

    CONSTRAINT FK_DeliveryMethodOrders_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
);

CREATE INDEX IX_DeliveryMethodOrders_OrderId ON DeliveryMethodOrders(OrderId);


CREATE TABLE Invoices (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Number VARCHAR(25) NOT NULL,
    DateOfIssue DATETIME NOT NULL,
    PaymentId INT NULL,
    IsPaid BIT NOT NULL,
    OrderId INT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Invoices_Customer FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id) ON DELETE NO ACTION,

    CONSTRAINT FK_Invoices_Payment FOREIGN KEY (PaymentId)
        REFERENCES Payments(Id) ON DELETE NO ACTION,

    CONSTRAINT FK_Invoices_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
CREATE INDEX IX_Invoices_PaymentId ON Invoices(PaymentId);


CREATE TABLE InvoiceItems (
    Id INT IDENTITY PRIMARY KEY,
    InvoiceId INT NOT NULL,
    ProductId INT NOT NULL,
    BasePricePerUnit DECIMAL(18,0) NOT NULL,
    TaxRate FLOAT NOT NULL,
    Quantity FLOAT NOT NULL,
    Discount FLOAT NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_InvoiceItems_Invoice FOREIGN KEY (InvoiceId)
        REFERENCES Invoices(Id) ON DELETE CASCADE,

    CONSTRAINT FK_InvoiceItems_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);
CREATE INDEX IX_InvoiceItems_ProductId ON InvoiceItems(ProductId);


CREATE TABLE Shipments (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    DeliveryMethodId INT NOT NULL,
    ShipmentDate DATETIME DEFAULT GETDATE(),
    EstimatedArrivalDate DATETIME NOT NULL,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Shipments_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),

    CONSTRAINT FK_Shipments_Method FOREIGN KEY (DeliveryMethodId)
        REFERENCES DeliveryMethods(Id)
);

CREATE INDEX IX_Shipments_OrderId ON Shipments(OrderId);
CREATE INDEX IX_Shipments_DeliveryMethodId ON Shipments(DeliveryMethodId);


CREATE TABLE ShoppingCartItems (
    Id INT IDENTITY PRIMARY KEY,
    ProductId INT NOT NULL,
    Amount INT NOT NULL,
    ShoppingCartId NVARCHAR(350) NOT NULL,
    UserId nvarchar(50) NULL,
    LastActivity datetime2 NULL,
    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_ShoppingCartItems_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id)
);

CREATE INDEX IX_ShoppingCartItems_ProductId ON ShoppingCartItems(ProductId);
CREATE INDEX IX_ShoppingCartItems_UserId ON ShoppingCartItems(UserId);
CREATE INDEX IX_ShoppingCartItems_ShoppingCartId ON ShoppingCartItems(ShoppingCartId);


GO
CREATE TABLE ShopCoins (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Balance INT DEFAULT 0,

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_ShopCoins_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_ShopCoins_CustomerId
ON ShopCoins(CustomerId);


GO
CREATE TABLE ShopCoinTransactionHistory (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Coins INT NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    OrderId INT NULL,
    IsActive BIT DEFAULT 1,

    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_ShopCoinTH_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id) ON DELETE CASCADE,

    CONSTRAINT FK_ShopCoinTH_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE NO ACTION
);


GO
CREATE TABLE ShopCoinSettings (
    Id INT PRIMARY KEY,
    EarnRate DECIMAL(10,2),
    SpendRate DECIMAL(10,2),
    MaxSpendPercentage DECIMAL(5,2),

    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL
);


GO
CREATE TABLE Notifications (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Message NVARCHAR(500),
    IsRead BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,

    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);


GO
CREATE TABLE Refunds (
    Id INT IDENTITY PRIMARY KEY,
    PaymentId INT NULL,
    CustomerId INT NOT NULL,
    Amount MONEY NOT NULL,
    Code NVARCHAR(255) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected
    IsActive BIT DEFAULT 1,
    ProcessedDate DATETIME NULL,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Refunds_Payment FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
    CONSTRAINT FK_Refunds_Customer FOREIGN KEY (CustomerId) REFERENCES Customers(Id)

);

ALTER TABLE Refunds
ADD CONSTRAINT UQ_Refunds_Code UNIQUE (Code);

CREATE UNIQUE INDEX IX_Refunds_ReturnId ON Refunds(ReturnId);
CREATE INDEX IX_Refunds_CustomerId ON Refunds(CustomerId);
CREATE INDEX IX_Refunds_Status ON Refunds(Status);
CREATE INDEX IX_Refunds_DateCreated ON Refunds(DateCreated DESC);

-- Create RefundItems Table
GO
CREATE TABLE [dbo].[RefundItems] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RefundId] INT NOT NULL,
    [OrderItemId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [RefundAmount] DECIMAL(18,2) NOT NULL,
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DateUpdated] DATETIME2 NULL,
    [DateDeleted] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_RefundItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefundItems_Refunds_RefundId] FOREIGN KEY ([RefundId]) 
        REFERENCES [dbo].[Refunds] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RefundItems_OrderItems_OrderItemId] FOREIGN KEY ([OrderItemId]) 
        REFERENCES [dbo].[OrderItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [CHK_RefundItems_Quantity] CHECK ([Quantity] > 0),
    CONSTRAINT [CHK_RefundItems_RefundAmount] CHECK ([RefundAmount] > 0),
    CONSTRAINT [CHK_RefundItems_TaxAmount] CHECK ([TaxAmount] >= 0)
);

GO
CREATE TABLE Returns (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    CustomerId INT NOT NULL,
    Reason NVARCHAR(500),
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected
    RefundAmount MONEY,
    IsActive BIT DEFAULT 1,

    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    CONSTRAINT FK_Returns_Order FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_Returns_Customer FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

CREATE INDEX IX_Returns_OrderId ON Returns(OrderId);
CREATE INDEX IX_Returns_CustomerId ON Returns(CustomerId);

GO
CREATE TABLE OrderStatusHistory (
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    Status NVARCHAR(50),
    ChangedBy NVARCHAR(100)  NOT NULL,
    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

GO
CREATE TABLE RefundStatusHistory (
    Id INT IDENTITY PRIMARY KEY,
    RefundId INT NOT NULL,
    Status NVARCHAR(50),
    RefundCode NVARCHAR(250),
    ChangedBy NVARCHAR(100)  NOT NULL,
    IsActive BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateUpdated DATETIME NULL,
    DateDeleted DATETIME NULL

    FOREIGN KEY (RefundId) REFERENCES Refunds(Id)
);

CREATE UNIQUE INDEX IX_Refunds_Code
ON Refunds(Code);

CREATE INDEX IX_Refunds_CustomerId
ON Refunds(CustomerId);

CREATE INDEX IX_Refunds_IsActive
ON Refunds(IsActive);

CREATE INDEX IX_Refunds_CustomerId_IsActive
ON Refunds(CustomerId, IsActive);

CREATE INDEX IX_Refunds_DateCreated
ON Refunds(DateCreated DESC);

ALTER TABLE Refunds
ADD CONSTRAINT UQ_Refunds_Code UNIQUE (Code);

ALTER TABLE Refunds
ADD CONSTRAINT CK_Refunds_Status
CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Cancelled'));

ALTER TABLE Refunds
ADD CONSTRAINT CK_Refunds_Amount
CHECK (Amount >= 0);

CREATE INDEX IX_RefundStatusHistory_RefundCode
ON RefundStatusHistory(RefundCode);

CREATE INDEX IX_RefundStatusHistory_DateCreated
ON RefundStatusHistory(DateCreated DESC);


ALTER TABLE RefundStatusHistory
ADD CONSTRAINT FK_RefundStatusHistory_Refunds
FOREIGN KEY (RefundCode)
REFERENCES Refunds(Code)
ON DELETE CASCADE;

ALTER TABLE RefundStatusHistory
ADD CONSTRAINT CK_RefundStatusHistory_Status
CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Cancelled'));

CREATE INDEX IX_Notifications_CustomerId
ON Notifications(CustomerId);

CREATE INDEX IX_Notifications_UserId
ON Notifications(UserId);

CREATE INDEX IX_Notifications_DateCreated
ON Notifications(DateCreated DESC);

CREATE INDEX IX_Notifications_IsRead
ON Notifications(IsRead);

CREATE INDEX IX_Notifications_CustomerId_IsRead_Date
ON Notifications(CustomerId, IsRead, DateCreated DESC);

ALTER TABLE Notifications
ADD CONSTRAINT CK_Notifications_Target
CHECK (
    CustomerId IS NOT NULL OR UserId IS NOT NULL
);

ALTER TABLE Notifications
ADD CONSTRAINT CK_Notifications_Message_Length
CHECK (LEN(Message) > 0);

CREATE INDEX IX_RefundItems_RefundCode
ON RefundItems(RefundCode);

CREATE INDEX IX_RefundItems_OrderItemId
ON RefundItems(OrderItemId);

ALTER TABLE RefundItems
ADD CONSTRAINT FK_RefundItems_Refunds
FOREIGN KEY (RefundCode)
REFERENCES Refunds(Code)
ON DELETE CASCADE;

ALTER TABLE RefundItems
ADD CONSTRAINT CK_RefundItems_Quantity
CHECK (Quantity > 0);



----  Views  ---->

GO
CREATE VIEW vw_Customers_Active AS
SELECT * FROM Customers
WHERE IsActive = 1 AND DateDeleted IS NULL;

GO 

CREATE VIEW vw_Customers_Detailed AS
SELECT c.*, a.City, a.CountryId
FROM Customers c
LEFT JOIN Addresses a ON c.Id = a.CustomerId;



GO
CREATE VIEW vw_Countries_Active AS
SELECT * FROM Countries
WHERE IsActive = 1;

GO
CREATE VIEW vw_Countries_Detailed AS
SELECT c.*, COUNT(a.Id) AS AddressCount
FROM Countries c
LEFT JOIN Addresses a ON c.Id = a.CountryId
GROUP BY c.Id, c.CountryName, c.CountryCode, c.IsActive, c.DateCreated, c.DateUpdated, c.DateDeleted;


GO
CREATE VIEW vw_Addresses_Active AS
SELECT * FROM Addresses
WHERE IsActive = 1;

GO
CREATE VIEW vw_Addresses_Detailed AS
SELECT a.*, c.CountryName, cu.UserId
FROM Addresses a
LEFT JOIN Countries c ON a.CountryId = c.Id
LEFT JOIN Customers cu ON a.CustomerId = cu.Id;


GO
CREATE VIEW vw_Categories_Active AS
SELECT * FROM Categories WHERE IsActive = 1;

GO
CREATE VIEW vw_Categories_Detailed AS
SELECT c.*, COUNT(p.ProductId) AS ProductCount
FROM Categories c
LEFT JOIN ProductCategories p ON c.Id = p.CategoryId
GROUP BY c.Id, c.Code, c.Name, c.About, c.IsActive, c.DateCreated, c.DateUpdated, c.DateDeleted;


GO
CREATE VIEW vw_Subcategories_Active AS
SELECT * FROM Subcategories WHERE IsActive = 1;

GO
CREATE VIEW vw_Subcategories_Detailed AS
SELECT s.*, c.Name AS CategoryName
FROM Subcategories s
JOIN Categories c ON s.CategoryId = c.Id;


GO
CREATE VIEW vw_Products_Active AS
SELECT * FROM Products WHERE IsActive = 1;

GO
CREATE VIEW vw_Products_Detailed AS
SELECT p.*, s.Name AS SubcategoryName
FROM Products p
JOIN Subcategories s ON p.SubcategoryId = s.Id;


GO
CREATE VIEW vw_ProductCategories_Active AS
SELECT * FROM ProductCategories WHERE IsActive = 1;

GO
CREATE VIEW vw_ProductCategories_Detailed AS
SELECT pc.*, p.Name AS ProductName, c.Name AS CategoryName
FROM ProductCategories pc
JOIN Products p ON pc.ProductId = p.Id
JOIN Categories c ON pc.CategoryId = c.Id;


GO
CREATE VIEW vw_Orders_Active AS
SELECT * FROM Orders WHERE IsActive = 1;

GO
CREATE VIEW vw_Orders_Detailed AS
SELECT o.*, c.UserId
FROM Orders o
JOIN Customers c ON o.CustomerId = c.Id;


GO
CREATE VIEW vw_OrderItems_Active AS
SELECT * FROM OrderItems WHERE IsActive = 1;

GO
CREATE VIEW vw_OrderItems_Detailed AS
SELECT oi.*, p.Name AS ProductName, o.Code AS OrderCode
FROM OrderItems oi
JOIN Products p ON oi.ProductId = p.Id
JOIN Orders o ON oi.OrderId = o.Id;


GO
CREATE VIEW vw_Coupons_Active AS
SELECT * FROM Coupons WHERE IsActive = 1;

GO
CREATE VIEW vw_Coupons_Detailed AS
SELECT c.*, COUNT(oc.Id) AS TimesUsed
FROM Coupons c
LEFT JOIN OrderCoupons oc ON c.Id = oc.CouponId
GROUP BY c.Id, c.Code, c.Description, c.DiscountAmount, c.DiscountPercentage,
         c.MinOrderAmount, c.MaxDiscountAmount, c.StartDate, c.EndDate,
         c.UsageLimit, c.UsedCount, c.PerUserLimit,
         c.IsActive, c.DateCreated, c.DateUpdated, c.DateDeleted;


GO
CREATE VIEW vw_OrderCoupons_Active AS
SELECT * FROM OrderCoupons WHERE IsActive = 1;

GO
CREATE VIEW vw_OrderCoupons_Detailed AS
SELECT oc.*, o.Code AS OrderCode, c.Code AS CouponCode
FROM OrderCoupons oc
JOIN Orders o ON oc.OrderId = o.Id
JOIN Coupons c ON oc.CouponId = c.Id;


GO
CREATE VIEW vw_PaymentMethods_Active AS
SELECT * FROM PaymentMethods WHERE IsActive = 1;

GO
CREATE VIEW vw_PaymentMethods_Detailed AS
SELECT pm.*, COUNT(p.Id) AS UsageCount
FROM PaymentMethods pm
LEFT JOIN Payments p ON pm.Id = p.PaymentMethodId
GROUP BY pm.Id, pm.PaymentType, pm.Details,
         pm.IsActive, pm.DateCreated, pm.DateUpdated, pm.DateDeleted;


GO
CREATE VIEW vw_Payments_Active AS
SELECT * FROM Payments WHERE IsActive = 1;

GO
CREATE VIEW vw_Payments_Detailed AS
SELECT p.*, o.Code AS OrderCode, pm.PaymentType
FROM Payments p
LEFT JOIN Orders o ON p.OrderId = o.Id
LEFT JOIN PaymentMethods pm ON p.PaymentMethodId = pm.Id;


GO
CREATE VIEW vw_Reviews_Active AS
SELECT * FROM Reviews WHERE IsActive = 1;

GO
CREATE VIEW vw_Reviews_Detailed AS
SELECT r.*, p.Name AS ProductName, c.UserId
FROM Reviews r
JOIN Products p ON r.ProductId = p.Id
JOIN Customers c ON r.CustomerId = c.Id;


GO
CREATE VIEW vw_Wishlists_Active AS
SELECT * FROM Wishlists WHERE IsActive = 1;

GO
CREATE VIEW vw_Wishlists_Detailed AS
SELECT w.*, c.UserId
FROM Wishlists w
JOIN Customers c ON w.CustomerId = c.Id;


GO
CREATE VIEW vw_WishlistItems_Active AS
SELECT * FROM WishlistItems WHERE IsActive = 1;

GO
CREATE VIEW vw_WishlistItems_Detailed AS
SELECT wi.*, p.Name AS ProductName, w.Name AS WishlistName
FROM WishlistItems wi
JOIN Products p ON wi.ProductId = p.Id
JOIN Wishlists w ON wi.WishlistId = w.Id;


GO
CREATE VIEW vw_DeliveryMethods_Active AS
SELECT * FROM DeliveryMethods WHERE IsActive = 1;

GO
CREATE VIEW vw_DeliveryMethods_Detailed AS
SELECT dm.*, COUNT(dmo.OrderId) AS UsedInOrders
FROM DeliveryMethods dm
LEFT JOIN DeliveryMethodOrders dmo ON dm.Id = dmo.DeliveryMethodId
GROUP BY dm.Id, dm.MethodName, dm.Cost,
         dm.IsActive, dm.DateCreated, dm.DateUpdated, dm.DateDeleted;


GO
CREATE VIEW vw_DeliveryMethodOrders_Active AS
SELECT * FROM DeliveryMethodOrders WHERE IsActive = 1;

GO
CREATE VIEW vw_DeliveryMethodOrders_Detailed AS
SELECT dmo.*, o.Code AS OrderCode, dm.MethodName
FROM DeliveryMethodOrders dmo
JOIN Orders o ON dmo.OrderId = o.Id
JOIN DeliveryMethods dm ON dmo.DeliveryMethodId = dm.Id;


GO
CREATE VIEW vw_Invoices_Active AS
SELECT * FROM Invoices WHERE IsActive = 1;

GO
CREATE VIEW vw_Invoices_Detailed AS
SELECT i.*, c.UserId, o.Code AS OrderCode
FROM Invoices i
JOIN Customers c ON i.CustomerId = c.Id
JOIN Orders o ON i.OrderId = o.Id;


GO
CREATE VIEW vw_InvoiceItems_Active AS
SELECT * FROM InvoiceItems WHERE IsActive = 1;

GO
CREATE VIEW vw_InvoiceItems_Detailed AS
SELECT ii.*, p.Name AS ProductName
FROM InvoiceItems ii
JOIN Products p ON ii.ProductId = p.Id;


GO
CREATE VIEW vw_Shipments_Active AS
SELECT * FROM Shipments WHERE IsActive = 1;

GO
CREATE VIEW vw_Shipments_Detailed AS
SELECT s.*, o.Code AS OrderCode, dm.MethodName
FROM Shipments s
JOIN Orders o ON s.OrderId = o.Id
JOIN DeliveryMethods dm ON s.DeliveryMethodId = dm.Id;


GO
CREATE VIEW vw_ShoppingCartItems_Active AS
SELECT * FROM ShoppingCartItems WHERE IsActive = 1;

GO
CREATE VIEW vw_ShoppingCartItems_Detailed AS
SELECT sci.*, p.Name AS ProductName, p.Price
FROM ShoppingCartItems sci
JOIN Products p ON sci.ProductId = p.Id;


GO
CREATE VIEW vw_CustomerRefundsSummary AS
SELECT 
    r.CustomerId,
    COUNT(*) AS TotalRefunds,
    SUM(r.Amount) AS TotalRefundAmount,
    MAX(r.DateCreated) AS LastRefundDate
FROM Refunds r
GROUP BY r.CustomerId;


GO
CREATE VIEW vw_RefundWithLatestStatus AS
SELECT r.*,
       h.Status AS LatestStatus,
       h.DateCreated AS StatusDate
FROM Refunds r
OUTER APPLY (
    SELECT TOP 1 *
    FROM RefundStatusHistory h
    WHERE h.RefundCode = r.Code
    ORDER BY h.DateCreated DESC
) h;


----  Functions  ---->

GO
CREATE FUNCTION fn_TotalRefundAmount (@CustomerId INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    RETURN (
        SELECT ISNULL(SUM(Amount), 0)
        FROM Refunds
        WHERE CustomerId = @CustomerId
    );
END;


GO
CREATE FUNCTION fn_CanCancelRefund (@Status NVARCHAR(50))
RETURNS BIT
AS
BEGIN
    RETURN (
        CASE 
            WHEN @Status IN ('Pending', 'Approved') THEN 1
            ELSE 0
        END
    );
END;

GO
CREATE FUNCTION fn_Customers_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Customers
WHERE IsActive = 1 AND DateDeleted IS NULL;

GO
CREATE FUNCTION fn_Customers_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Customers WHERE Id = @Id;


GO
CREATE FUNCTION fn_Countries_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Countries WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Countries_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Countries WHERE Id = @Id;


GO
CREATE FUNCTION fn_Addresses_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Addresses WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Addresses_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Addresses WHERE Id = @Id;


GO
CREATE FUNCTION fn_Categories_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Categories WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Categories_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Categories WHERE Id = @Id;


GO
CREATE FUNCTION fn_Subcategories_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Subcategories WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Subcategories_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Subcategories WHERE Id = @Id;


GO
CREATE FUNCTION fn_Products_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Products WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Products_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Products WHERE Id = @Id;


GO
CREATE FUNCTION fn_ProductCategories_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM ProductCategories WHERE IsActive = 1;

GO
CREATE FUNCTION fn_ProductCategories_ById(@ProductId INT, @CategoryId INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM ProductCategories 
WHERE ProductId = @ProductId AND CategoryId = @CategoryId;


GO
CREATE FUNCTION fn_Orders_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Orders WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Orders_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Orders WHERE Id = @Id;


GO
CREATE FUNCTION fn_OrderItems_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM OrderItems WHERE IsActive = 1;

GO
CREATE FUNCTION fn_OrderItems_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM OrderItems WHERE Id = @Id;


GO
CREATE FUNCTION fn_Coupons_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Coupons WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Coupons_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Coupons WHERE Id = @Id;


GO
CREATE FUNCTION fn_OrderCoupons_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM OrderCoupons WHERE IsActive = 1;

GO
CREATE FUNCTION fn_OrderCoupons_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM OrderCoupons WHERE Id = @Id;


GO
CREATE FUNCTION fn_PaymentMethods_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM PaymentMethods WHERE IsActive = 1;

GO
CREATE FUNCTION fn_PaymentMethods_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM PaymentMethods WHERE Id = @Id;


GO
CREATE FUNCTION fn_Payments_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Payments WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Payments_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Payments WHERE Id = @Id;


GO
CREATE FUNCTION fn_Reviews_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Reviews WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Reviews_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Reviews WHERE Id = @Id;


GO
CREATE FUNCTION fn_Wishlists_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Wishlists WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Wishlists_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Wishlists WHERE Id = @Id;


GO
CREATE FUNCTION fn_WishlistItems_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM WishlistItems WHERE IsActive = 1;

GO
CREATE FUNCTION fn_WishlistItems_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM WishlistItems WHERE Id = @Id;


GO
CREATE FUNCTION fn_DeliveryMethods_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM DeliveryMethods WHERE IsActive = 1;

GO
CREATE FUNCTION fn_DeliveryMethods_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM DeliveryMethods WHERE Id = @Id;


GO
CREATE FUNCTION fn_DeliveryMethodOrders_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM DeliveryMethodOrders WHERE IsActive = 1;

GO
CREATE FUNCTION fn_DeliveryMethodOrders_ById(@DeliveryMethodId INT, @OrderId INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM DeliveryMethodOrders
WHERE DeliveryMethodId = @DeliveryMethodId AND OrderId = @OrderId;


GO
CREATE FUNCTION fn_Invoices_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Invoices WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Invoices_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Invoices WHERE Id = @Id;


GO
CREATE FUNCTION fn_InvoiceItems_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM InvoiceItems WHERE IsActive = 1;

GO
CREATE FUNCTION fn_InvoiceItems_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM InvoiceItems WHERE Id = @Id;


GO
CREATE FUNCTION fn_Shipments_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM Shipments WHERE IsActive = 1;

GO
CREATE FUNCTION fn_Shipments_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM Shipments WHERE Id = @Id;


GO
CREATE FUNCTION fn_ShoppingCartItems_GetActive()
RETURNS TABLE
AS
RETURN
SELECT * FROM ShoppingCartItems WHERE IsActive = 1;

GO
CREATE FUNCTION fn_ShoppingCartItems_ById(@Id INT)
RETURNS TABLE
AS
RETURN
SELECT * FROM ShoppingCartItems WHERE Id = @Id;


-- Procedures -->
GO
CREATE PROCEDURE sp_GetCustomerProfile
    @CustomerId INT
AS
BEGIN
    SELECT 
        c.*,
        dbo.fn_GetCustomerPoints(@CustomerId) AS PointsCalculated
    FROM vw_Customers_Active c
    WHERE c.Id = @CustomerId;
END


GO
CREATE PROCEDURE sp_UpdateCustomerPoints
    @CustomerId INT,
    @Points INT
AS
BEGIN
    UPDATE Customers
    SET Points = @Points,
        DateUpdated = GETDATE()
    WHERE Id = @CustomerId;
END


GO
CREATE PROCEDURE sp_GetCustomerAddresses
    @CustomerId INT
AS
BEGIN
    SELECT *
    FROM vw_Addresses_WithCountry
    WHERE CustomerId = @CustomerId;
END


GO
CREATE PROCEDURE sp_AddAddress
    @CustomerId INT,
    @CountryId INT,
    @Street VARCHAR(100),
    @City VARCHAR(50)
AS
BEGIN
    INSERT INTO Addresses (CustomerId, CountryId, Street, City)
    VALUES (@CustomerId, @CountryId, @Street, @City);
END


GO
CREATE PROCEDURE sp_GetProductDetails
    @ProductId INT
AS
BEGIN
    SELECT *
    FROM vw_Products_Full
    WHERE Id = @ProductId;
END


GO
CREATE PROCEDURE sp_UpdateProductStock
    @ProductId INT,
    @Stock INT
AS
BEGIN
    UPDATE Products
    SET Stock = @Stock,
        DateUpdated = GETDATE()
    WHERE Id = @ProductId;
END


GO
CREATE PROCEDURE sp_GetOrderDetails
    @OrderId INT
AS
BEGIN
    SELECT 
        o.*,
        dbo.fn_GetOrderTotal(@OrderId) AS CalculatedTotal
    FROM vw_Orders_Details o
    WHERE o.Id = @OrderId;
END


GO
CREATE PROCEDURE sp_CreateOrder
    @CustomerId INT,
    @TotalAmount MONEY
AS
BEGIN
    INSERT INTO Orders (CustomerId, TotalAmount, IsPaid)
    VALUES (@CustomerId, @TotalAmount, 0);
END


GO
CREATE PROCEDURE sp_AddOrderItem
    @OrderId INT,
    @ProductId INT,
    @Quantity INT,
    @UnitPrice MONEY
AS
BEGIN
    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
END


GO
CREATE PROCEDURE sp_GetOrderItems
    @OrderId INT
AS
BEGIN
    SELECT *
    FROM vw_OrderItems_Detailed
    WHERE OrderId = @OrderId;
END


GO
CREATE PROCEDURE sp_CreatePayment
    @OrderId INT,
    @PaymentMethodId INT,
    @Amount MONEY
AS
BEGIN
    INSERT INTO Payments (OrderId, PaymentMethodId, Amount)
    VALUES (@OrderId, @PaymentMethodId, @Amount);
END


GO
CREATE PROCEDURE sp_GetPaymentsByOrder
    @OrderId INT
AS
BEGIN
    SELECT *
    FROM vw_Payments_Detailed
    WHERE OrderId = @OrderId;
END


GO
CREATE PROCEDURE sp_ApplyCoupon
    @OrderId INT,
    @CouponId INT
AS
BEGIN
    DECLARE @Discount DECIMAL(18,2);

    SET @Discount = dbo.fn_CalculateCouponDiscount(@OrderId, @CouponId);

    INSERT INTO OrderCoupons (OrderId, CouponId, DiscountApplied)
    VALUES (@OrderId, @CouponId, @Discount);
END


GO
CREATE PROCEDURE sp_GetActiveCoupons
AS
BEGIN
    SELECT *
    FROM vw_Coupons_Active;
END


GO
CREATE PROCEDURE sp_AddToWishlist
    @WishlistId INT,
    @ProductId INT
AS
BEGIN
    INSERT INTO WishlistItems (WishlistId, ProductId)
    VALUES (@WishlistId, @ProductId);
END


GO
CREATE PROCEDURE sp_GetWishlist
    @WishlistId INT
AS
BEGIN
    SELECT *
    FROM vw_WishlistItems_Detailed
    WHERE WishlistId = @WishlistId;
END


GO
CREATE PROCEDURE sp_CreateShipment
    @OrderId INT,
    @DeliveryMethodId INT,
    @EstimatedArrival DATETIME
AS
BEGIN
    INSERT INTO Shipments (OrderId, DeliveryMethodId, EstimatedArrivalDate)
    VALUES (@OrderId, @DeliveryMethodId, @EstimatedArrival);
END


GO
CREATE PROCEDURE sp_GetShipment
    @OrderId INT
AS
BEGIN
    SELECT *
    FROM vw_Shipments_Detailed
    WHERE OrderId = @OrderId;
END


GO
CREATE PROCEDURE sp_CreateInvoice
    @CustomerId INT,
    @OrderId INT,
    @PaymentId INT,
    @Number VARCHAR(25)
AS
BEGIN
    INSERT INTO Invoices (CustomerId, OrderId, PaymentId, Number, DateOfIssue, IsPaid)
    VALUES (@CustomerId, @OrderId, @PaymentId, @Number, GETDATE(), 1);
END


GO
CREATE PROCEDURE sp_GetInvoice
    @InvoiceId INT
AS
BEGIN
    SELECT *
    FROM vw_Invoices_Detailed
    WHERE Id = @InvoiceId;
END


GO
CREATE PROCEDURE sp_AddToCart
    @ProductId INT,
    @Amount INT,
    @CartId NVARCHAR(50)
AS
BEGIN
    INSERT INTO ShoppingCartItems (ProductId, Amount, ShoppingCartId)
    VALUES (@ProductId, @Amount, @CartId);
END


GO
CREATE PROCEDURE sp_GetCart
    @CartId NVARCHAR(50)
AS
BEGIN
    SELECT *
    FROM vw_ShoppingCart_Detailed
    WHERE ShoppingCartId = @CartId;
END


GO
CREATE PROCEDURE sp_CreateRefund
    @CustomerId INT,
    @OrderCode NVARCHAR(100),
    @Amount DECIMAL(18,2)
AS
BEGIN
    INSERT INTO Refunds (CustomerId, Code, Amount, Status, DateCreated)
    VALUES (@CustomerId, NEWID(), @Amount, 'Pending', GETDATE());
END;


GO
CREATE PROCEDURE sp_ChangeRefundStatus
    @RefundCode NVARCHAR(100),
    @Status NVARCHAR(50)
AS
BEGIN
    UPDATE Refunds
    SET Status = @Status,
        ProcessedDate = CASE 
            WHEN @Status IN ('Approved', 'Rejected') THEN GETDATE()
            ELSE ProcessedDate
        END
    WHERE Code = @RefundCode;

    INSERT INTO RefundStatusHistory (RefundCode, Status, DateCreated)
    VALUES (@RefundCode, @Status, GETDATE());
END;


GO
CREATE PROCEDURE sp_CancelRefund
    @RefundCode NVARCHAR(100)
AS
BEGIN
    UPDATE Refunds
    SET Status = 'Cancelled'
    WHERE Code = @RefundCode;

    INSERT INTO RefundStatusHistory (RefundCode, Status, DateCreated)
    VALUES (@RefundCode, 'Cancelled', GETDATE());
END;


Go
CREATE PROCEDURE sp_MarkNotificationAsRead
    @NotificationId INT
AS
BEGIN
    UPDATE Notifications
    SET IsRead = 1
    WHERE Id = @NotificationId;
END;



ALTER TABLE Notifications
ADD CONSTRAINT CK_Notification_Target
CHECK (
    CustomerId IS NOT NULL OR UserId IS NOT NULL
);

