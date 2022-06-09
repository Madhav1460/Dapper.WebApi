Create procedure usp_getProducts
As
Begin
  Select * from [dbo].[Products]
End

GO
Create procedure usp_getProduct
@ProductId int
As
Begin
  Select * from [dbo].[Products] where Id=@ProductId
End

GO
Create procedure usp_addProduct
@Name varchar(50),
@Barcode varchar(50),
@Description varchar(max),
@Rate decimal(18,2),
@AddedOn datetime
As
Begin
  Insert into [Products] (Name, Barcode, Description, Rate,AddedOn)
VALUES
	(@Name, @Barcode, @Description, @Rate, @AddedOn)
End

GO
Create procedure usp_getProductCategories
@ProductId int
As
Begin
  Select * from [dbo].[Products] where Id=@ProductId
  Select * from [dbo].[Categories]
End