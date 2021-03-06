USE [master]
GO
/****** Object:  Database [wiadvice_siteDB_EN]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE DATABASE [wiadvice_siteDB_EN]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'wiadvice_siteDB_EN', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_siteDB_EN.mdf' , SIZE = 10432KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'wiadvice_siteDB_EN_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_siteDB_EN.ldf' , SIZE = 5824KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET COMPATIBILITY_LEVEL = 90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [wiadvice_siteDB_EN].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ARITHABORT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET  DISABLE_BROKER 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET  MULTI_USER 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET DB_CHAINING OFF 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [wiadvice_siteDB_EN]
GO
/****** Object:  User [wiadvice_EnAdmin]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE USER [wiadvice_EnAdmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[wiadvice_EnAdmin]
GO
ALTER ROLE [db_owner] ADD MEMBER [wiadvice_EnAdmin]
GO
/****** Object:  Schema [wiadvice_EnAdmin]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE SCHEMA [wiadvice_EnAdmin]
GO


/****** Object:  StoredProcedure [dbo].[FindCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCategories] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindCategories] @searchKeywords, @fromIndex, @toIndex, 0
END

END
GO
/****** Object:  StoredProcedure [dbo].[FindCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCompanies] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindCompanies] @searchKeywords, @fromIndex, @toIndex, 0
END
END

GO
/****** Object:  StoredProcedure [dbo].[FindCompaniesWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCompaniesWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindAlternativeCompanies] @searchKeywords, @fromIndex, @toIndex, 0
END
END

GO
/****** Object:  StoredProcedure [dbo].[FindCompaniesWithType]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCompaniesWithType] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@typeID BIGINT
AS
BEGIN
	BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@typeID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@typeID')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindCompaniesWithType] @searchKeywords, @fromIndex, @toIndex, @typeID, 0
END
END
GO
/****** Object:  StoredProcedure [dbo].[FindCountCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountCategories] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindCategories] @searchKeywords, @fromIndex, @toIndex, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountCompanies] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindCompanies] @searchKeywords, @fromIndex, @toIndex, 1
END


GO
/****** Object:  StoredProcedure [dbo].[FindCountCompaniesWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountCompaniesWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindAlternativeCompanies] @searchKeywords, @fromIndex, @toIndex, 1
END


GO
/****** Object:  StoredProcedure [dbo].[FindCountCompaniesWithType]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountCompaniesWithType] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@typeID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@typeID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@typeID')
		RETURN
	END
	
	EXECUTE [InternalFindCompaniesWithType] @searchKeywords, @fromIndex, @toIndex, @typeID, 1
END
GO
/****** Object:  StoredProcedure [dbo].[FindCountProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProducts] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProducts] @searchKeywords, @fromIndex, @toIndex, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE InternalFindProductsInCategory @searchKeywords, @fromIndex, @toIndex, @catID, 1
END


GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsInCategoryWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsInCategoryWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE InternalFindAlternativeProductsInCategory @searchKeywords, @fromIndex, @toIndex, @catID, 1
END


GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsSubvariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsSubvariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductSubVariants] @searchKeywords, @fromIndex, @toIndex, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsSubvariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsSubvariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@catID BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductSubVariantsInCategory] @searchKeywords, @fromIndex, @toIndex, @catID, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsVariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductVariants] @searchKeywords, @fromIndex, @toIndex, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsVariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsVariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@catID BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductVariantsInCategory] @searchKeywords, @fromIndex, @toIndex, @catID, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountProductsWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountProductsWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindAlternativeProducts] @searchKeywords, @fromIndex, @toIndex, 1
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountUsers]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCountUsers] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindUsers] @searchKeywords, @fromIndex, @toIndex, 1
END


GO
/****** Object:  StoredProcedure [dbo].[FindProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProducts] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProducts] @searchKeywords, @fromIndex, @toIndex, 0
END

GO
/****** Object:  StoredProcedure [dbo].[FindProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE InternalFindProductsInCategory @searchKeywords, @fromIndex, @toIndex, @catID, 0
END


GO
/****** Object:  StoredProcedure [dbo].[FindProductsInCategoryWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsInCategoryWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE InternalFindAlternativeProductsInCategory @searchKeywords, @fromIndex, @toIndex, @catID, 0
END


GO
/****** Object:  StoredProcedure [dbo].[FindProductsSubVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsSubVariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductSubVariants] @searchKeywords, @fromIndex, @toIndex, 0
END

GO
/****** Object:  StoredProcedure [dbo].[FindProductsSubVariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsSubVariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductSubVariantsInCategory] @searchKeywords, @fromIndex, @toIndex, @catID, 0
END


GO
/****** Object:  StoredProcedure [dbo].[FindProductsVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsVariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductVariants] @searchKeywords, @fromIndex, @toIndex, 0
END

GO
/****** Object:  StoredProcedure [dbo].[FindProductsVariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsVariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindProductVariantsInCategory] @searchKeywords, @fromIndex, @toIndex, @catID, 0
END


GO
/****** Object:  StoredProcedure [dbo].[FindProductsWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindProductsWithAlternativeNames] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindAlternativeProducts] @searchKeywords, @fromIndex, @toIndex, 0
END

GO
/****** Object:  StoredProcedure [dbo].[FindUsers]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindUsers] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	EXECUTE [dbo].[InternalFindUsers] @searchKeywords, @fromIndex, @toIndex, 0
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllCompanyProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllCompanyProductsInCategory]
	@compID BIGINT,
	@catID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@compID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@compID')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[companyID] = @compID AND p.[categoryID] = @catID 
	AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	
	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllOtherProductsFromCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllOtherProductsFromCategory]
	@catID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[categoryID] = @catID 
	AND (SUBSTRING(p.[name], 1, 1) NOT BETWEEN 'A' AND 'Z')
	AND (SUBSTRING(p.[name], 1, 1) NOT BETWEEN '0' AND '9')
	AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	

	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllProductsFromCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllProductsFromCategory]
	@catID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[categoryID] = @catID AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	

	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllProductsFromCategoryWhichStartWith]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllProductsFromCategoryWhichStartWith]
	@catID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@char CHAR(1)
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@char IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@char')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[categoryID] = @catID
	AND SUBSTRING(p.[name], 1, 1) = @char
	AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	

	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllProductsFromCategoryWhichStartWithNumber]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllProductsFromCategoryWhichStartWithNumber]
	@catID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[categoryID] = @catID
	AND SUBSTRING(p.[name], 1, 1) BETWEEN '0' AND '9'
	AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	

	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllProductsFromCompany]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllProductsFromCompany]
	@compID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@compID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@compID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @productsIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	DECLARE Products_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Products] p
	WHERE p.[companyID] = @compID AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Products_Cursor
	FETCH NEXT FROM Products_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @productsIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Products_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Products_Cursor
	DEALLOCATE Products_Cursor
	

	SELECT * FROM [dbo].[Products] WHERE ID IN (SELECT cID FROM @productsIDs) ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAutoCompleteCompanies]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Companies] c' +
	' WHERE c.name LIKE ''' + @searchCriterion + ''' AND c.visible = 1' +
	' AND c.ID != ' + CAST(@otherCompanyID AS NVARCHAR) +
	' ORDER BY c.name'
	EXECUTE (@sql)
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteCompaniesWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAutoCompleteCompaniesWithAlternativeNames]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	' SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[AlternativeCompanyNames] acn, [dbo].[Companies] c' +
	' WHERE acn.name LIKE ''' + @searchCriterion + ''' AND acn.visible = 1' +
	' AND c.[ID] = acn.[Company] AND [dbo].[AreCompanyRelationsOk](c.[ID]) = 1' +
	' ORDER BY acn.name'
	EXECUTE (@sql)
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAutoCompleteProducts]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	' SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Products] p' +
	' WHERE p.name LIKE ''' + @searchCriterion + ''' AND p.visible = 1' +
	' AND [dbo].[AreProductRelationsOk](p.[ID], ' + CAST(@otherCompanyID AS NVARCHAR) + ') = 1' +
	' ORDER BY p.name'
	EXECUTE (@sql)
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteProductsSubVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAutoCompleteProductsSubVariants]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	' SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[ProductSubVariants] psv, [dbo].[ProductVariants] pv, [dbo].[Products] p' +
	' WHERE psv.name LIKE ''' + @searchCriterion + ''' AND psv.visible = 1' +
	' AND pv.[ID] = psv.[Variant] AND pv.[visible] = 1 ' +
	' AND p.[ID] = pv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], ' + CAST(@otherCompanyID AS NVARCHAR) + ') = 1' +
	' AND p.[ID] = psv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], ' + CAST(@otherCompanyID AS NVARCHAR) + ') = 1' +
	' ORDER BY psv.name'
	EXECUTE (@sql)
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteProductsVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAutoCompleteProductsVariants]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	' SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[ProductVariants] pv, [dbo].[Products] p' +
	' WHERE pv.name LIKE ''' + @searchCriterion + ''' AND pv.visible = 1' +
	' AND p.[ID] = pv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], ' + CAST(@otherCompanyID AS NVARCHAR) + ') = 1' +
	' ORDER BY pv.name'
	EXECUTE (@sql)
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetAutoCompleteProductsWithAlternativeNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAutoCompleteProductsWithAlternativeNames]
	@search NVARCHAR(100),
	@count BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @searchCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	DECLARE @otherCompanyID BIGINT
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END

	SET @searchCriterion = '%' + @search + '%'
	SET @sql = 
	' SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[AlternativeProductNames] apn, [dbo].[Products] p' +
	' WHERE apn.name LIKE ''' + @searchCriterion + ''' AND apn.visible = 1' +
	' AND p.[ID] = apn.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], ' + CAST(@otherCompanyID AS NVARCHAR) + ') = 1' +
	' ORDER BY apn.name'
	EXECUTE (@sql)
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetAutocompleteResults]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAutocompleteResults]
	@search NVARCHAR(200)
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@search IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@search')
		RETURN
	END
	
	SELECT * FROM [dbo].[AutoCompleteSearch] WHERE [string] LIKE @search + '%'
	ORDER BY [foundResults] DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetCategorySubCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCategorySubCategories]
	@catID BIGINT,
	@sortedByASC BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	IF (@sortedByASC IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortedByASC')
		RETURN
	END

	IF(@sortedByASC = 1)
	BEGIN
		SELECT * FROM [dbo].[Categories] c 
		WHERE c.[parentID] = @catID AND c.[visible] = 1 
		ORDER BY displayOrder, name
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Categories] c 
		WHERE c.[parentID] = @catID AND c.[visible] = 1 
		ORDER BY ID
	END
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProduct]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProduct]
	@prodID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@prodID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@prodID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [typeID] = @prodID AND [type] = 'product' AND [subType] = 'comment' AND [visible] = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [type] = 'product' AND [typeID] = @prodID AND visible = 1 AND [subType] = 'comment'
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductByRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductByRating]
	@prodID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@prodID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@prodID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDsAndRatings TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @ID BIGINT
	DECLARE @rating INT
	DECLARE @index BIGINT
	
	INSERT INTO @commentIDsAndRatings(cID, cRating)
		(SELECT c.[ID], [dbo].[GetCommentRating](c.[ID])
			FROM [dbo].[Comments] c WHERE c.[type] = 'product' AND [subType] = 'comment' AND c.[typeID] = @prodID AND c.[visible] = 1)
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor_Desc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating DESC
	
	SET @index = 0
	OPEN Comments_Cursor_Desc
	FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Desc
	DEALLOCATE Comments_Cursor_Desc
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor_Asc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating ASC
	
	SET @index = 0
	OPEN Comments_Cursor_Asc
	FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Asc
	DEALLOCATE Comments_Cursor_Asc
	
	END
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] DESC
	END
	ELSE
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] ASC
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductCharacteristic]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductCharacteristic]
	@charID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@charID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [type] = 'product' AND [subType] = 'comment' AND [characteristicID] = @charID AND visible = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments]
	WHERE [type] = 'product' AND [subType] = 'comment' AND [characteristicID] = @charID AND visible = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductCharacteristicByRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductCharacteristicByRating]
	@charID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@charID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDsAndRatings TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @ID BIGINT
	DECLARE @rating INT
	DECLARE @index BIGINT
	
	INSERT INTO @commentIDsAndRatings(cID, cRating)
		(SELECT c.[ID], [dbo].[GetCommentRating](c.[ID])
			FROM [dbo].[Comments] c 
			WHERE c.[type] = 'product' AND [subType] = 'comment' AND c.[characteristicID] = @charID AND c.[visible] = 1)
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor_Desc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating DESC
	
	SET @index = 0
	OPEN Comments_Cursor_Desc
	FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Desc
	DEALLOCATE Comments_Cursor_Desc
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor_Asc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating ASC
	
	SET @index = 0
	OPEN Comments_Cursor_Asc
	FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Asc
	DEALLOCATE Comments_Cursor_Asc
	
	END
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] DESC
	END
	ELSE
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] ASC
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductSubVariant]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductSubVariant]
	@varID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@varID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [type] = 'product' AND [subType] = 'comment' AND [SubVariant] = @varID AND visible = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments]
	WHERE [type] = 'product' AND [subType] = 'comment' AND [SubVariant] = @varID AND visible = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductSubVariantByRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductSubVariantByRating]
	@varID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@varID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDsAndRatings TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @ID BIGINT
	DECLARE @rating INT
	DECLARE @index BIGINT
	
	INSERT INTO @commentIDsAndRatings(cID, cRating)
		(SELECT c.[ID], [dbo].[GetCommentRating](c.[ID])
			FROM [dbo].[Comments] c 
			WHERE c.[type] = 'product' AND [subType] = 'comment' AND c.[SubVariant] = @varID AND c.[visible] = 1)
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor_Desc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating DESC
	
	SET @index = 0
	OPEN Comments_Cursor_Desc
	FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Desc
	DEALLOCATE Comments_Cursor_Desc
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor_Asc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating ASC
	
	SET @index = 0
	OPEN Comments_Cursor_Asc
	FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Asc
	DEALLOCATE Comments_Cursor_Asc
	
	END
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] DESC
	END
	ELSE
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] ASC
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductVariant]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductVariant]
	@varID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@varID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [type] = 'product' AND [subType] = 'comment' AND [Variant] = @varID AND visible = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments]
	WHERE [type] = 'product' AND [subType] = 'comment' AND [Variant] = @varID AND visible = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductVariantByRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductVariantByRating]
	@varID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@varID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@charID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDsAndRatings TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @ID BIGINT
	DECLARE @rating INT
	DECLARE @index BIGINT
	
	INSERT INTO @commentIDsAndRatings(cID, cRating)
		(SELECT c.[ID], [dbo].[GetCommentRating](c.[ID])
			FROM [dbo].[Comments] c 
			WHERE c.[type] = 'product' AND [subType] = 'comment' AND c.[Variant] = @varID AND c.[visible] = 1)
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor_Desc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating DESC
	
	SET @index = 0
	OPEN Comments_Cursor_Desc
	FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Desc
	DEALLOCATE Comments_Cursor_Desc
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor_Asc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating ASC
	
	SET @index = 0
	OPEN Comments_Cursor_Asc
	FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Asc
	DEALLOCATE Comments_Cursor_Asc
	
	END
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] DESC
	END
	ELSE
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] ASC
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductWithNoAbout]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductWithNoAbout]
    @prodID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@prodID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@prodID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] 
	WHERE [typeID] = @prodID AND [type] = 'product' AND [subType] = 'comment' AND [characteristicID] IS NULL 
	AND [Variant] IS NULL AND [SubVariant] IS NULL AND [visible] = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments]
	WHERE [typeID] = @prodID AND [type] = 'product' AND [subType] = 'comment' AND [characteristicID] IS NULL 
	AND [Variant] IS NULL AND [SubVariant] IS NULL AND [visible] = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromProductWithNoAboutByRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromProductWithNoAboutByRating]
	@prodID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@prodID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@prodID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDsAndRatings TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY, cRating INT NOT NULL)
	DECLARE @ID BIGINT
	DECLARE @rating INT
	DECLARE @index BIGINT
	
	INSERT INTO @commentIDsAndRatings(cID, cRating)
		(SELECT c.[ID], [dbo].[GetCommentRating](c.[ID])
			FROM [dbo].[Comments] c 
			WHERE c.[typeID] = @prodID AND c.[type] = 'product' AND c.[subType] = 'comment' AND c.[characteristicID] IS NULL 
			AND c.[Variant] IS NULL AND c.[SubVariant] IS NULL AND c.[visible] = 1)
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor_Desc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating DESC
	
	SET @index = 0
	OPEN Comments_Cursor_Desc
	FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Desc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Desc
	DEALLOCATE Comments_Cursor_Desc
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor_Asc CURSOR FOR
		SELECT cID, cRating FROM @commentIDsAndRatings ORDER BY cRating ASC
	
	SET @index = 0
	OPEN Comments_Cursor_Asc
	FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID, cRating) VALUES(@ID, @rating)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor_Asc INTO @ID, @rating
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor_Asc
	DEALLOCATE Comments_Cursor_Asc
	
	END
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] DESC
	END
	ELSE
	BEGIN
		SELECT comment.* FROM [dbo].[Comments] comment, @commentIDs i WHERE comment.[ID] = i.[cID] ORDER BY i.[cRating] ASC
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsFromUser]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsFromUser]
	@userID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sortByDesc BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@userID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@userID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sortByDesc IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sortByDesc')
		RETURN
	END
	
	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sortByDesc = 1)
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] WHERE [userID] = @userID AND visible = 1
	ORDER BY ID DESC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments]
	WHERE [userID] = @userID AND visible = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor
	
	END
	
	
	IF (@sortByDesc = 1)
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetCommentsRange]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsRange]
	@type NVARCHAR(100),
	@typeID BIGINT,
	@fromIndex INT,
	@toIndex INT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@type IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@type')
		RETURN
	END
	IF (@typeID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@typeID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END

	DECLARE @commentIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index INT
	
	SET @index = 0
	
	DECLARE Comments_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[Comments] WHERE [type] = @type AND [typeID] = @typeID AND [subType] = 'comment' AND visible = 1
		
	OPEN Comments_Cursor
	FETCH NEXT FROM Comments_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index <= @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @commentIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Comments_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Comments_Cursor
	DEALLOCATE Comments_Cursor

	SELECT * FROM [dbo].[Comments] WHERE ID IN (SELECT cID FROM @commentIDs)
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastAddedCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastAddedCompanies]
	@number BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Companies] c
	WHERE c.[visible] = 1 AND c.[ID] != @otherCompanyID
	ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastAddedProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastAddedProducts]
	@number BIGINT,
	@withImages BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	IF (@withImages IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@withImages')
		RETURN
	END
	

	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (@withImages = 1)
	BEGIN
	
	SELECT TOP (@number) * FROM [dbo].[Products] p
	WHERE [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 AND 
	((SELECT COUNT(ID) FROM [dbo].[ProductImages] i WHERE i.[prodID] = p.[ID] AND i.[isThumbnail] = 1) > 0)
	ORDER BY ID DESC
	
	END
	ELSE
	BEGIN
	
	SELECT TOP (@number) * FROM [dbo].[Products] p
	WHERE [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	ORDER BY ID DESC
	
	END
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastAddedProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastAddedProductsInCategory]
	@number BIGINT,
	@catID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Products] p
	WHERE p.categoryID = @catID AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	ORDER BY ID DESC
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetLastComments]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastComments]
	@number INT,
	@ipAdress NVARCHAR(100),
	@maxLength INT,
	@all BIT,
	@onlyVisibleTrue BIT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	IF (@ipAdress IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@ipAdress')
		RETURN
	END
	
	IF (@maxLength IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@maxLength')
		RETURN
	END
	
	IF (@all IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@all')
		RETURN
	END
	
	IF (@onlyVisibleTrue IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@onlyVisibleTrue')
		RETURN
	END

	IF (@ipAdress = 'null')
	BEGIN
		
		IF (@all = '1')
		BEGIN
			SELECT TOP (@number) * FROM [dbo].[Comments] c
			WHERE DATALENGTH(c.[description]) <= @maxLength
			ORDER BY ID DESC
		END
		ELSE
		BEGIN
			IF (@onlyVisibleTrue = '1')
			BEGIN	
				SELECT TOP (@number) * FROM [dbo].[Comments] c
				WHERE DATALENGTH(c.[description]) <= @maxLength AND c.[visible] = 1
				ORDER BY ID DESC
			END
			ELSE
			BEGIN
				SELECT TOP (@number) * FROM [dbo].[Comments] c
				WHERE DATALENGTH(c.[description]) <= @maxLength AND c.[visible] = 0
				ORDER BY ID DESC
			END
		END
		
	END
	ELSE
	BEGIN
		IF (@all = '1')
		BEGIN
			SELECT TOP (@number) * FROM [dbo].[Comments] c
			WHERE DATALENGTH(c.[description]) <= @maxLength AND c.[ipAdress] = @ipAdress 
			ORDER BY ID DESC
		END
		ELSE
		BEGIN
			IF (@onlyVisibleTrue = '1')
			BEGIN	
				SELECT TOP (@number) * FROM [dbo].[Comments] c
				WHERE DATALENGTH(c.[description]) <= @maxLength AND c.[visible] = 1 AND c.[ipAdress] = @ipAdress
				ORDER BY ID DESC
			END
			ELSE
			BEGIN
				SELECT TOP (@number) * FROM [dbo].[Comments] c
				WHERE DATALENGTH(c.[description]) <= @maxLength AND c.[visible] = 0 AND c.[ipAdress] = @ipAdress
				ORDER BY ID DESC
			END
		END
		
	END
	
	
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastCompanies]
	@number BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Companies] c
	ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastDeletedCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastDeletedCategories]
	@nameContains NVARCHAR(100),
	@count BIGINT,
	@byUser BIGINT,
	@catID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @catCriterion NVARCHAR(150)
	DECLARE @userCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@nameContains IS NOT NULL AND @catID IS NOT NULL)
	BEGIN
		RAISERROR('nameContains is not null AND compID is not null', 16, 1)
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	IF(@nameContains IS NOT NULL)
	BEGIN
		SET @catCriterion = 'c.name LIKE ''%' + @nameContains + '%'' AND'
	END
	ELSE IF(@catID IS NOT NULL)
	BEGIN
		SET @catCriterion = 'c.ID = ' + CAST(@catID AS NVARCHAR) + ' AND'
	END
	ELSE
	BEGIN
		SET @catCriterion = ' '
	END

	IF(@byUser IS NOT NULL)
	BEGIN
		SET @userCriterion = ' AND c.lastModifiedBy = ' + CAST(@byUser AS NVARCHAR)
	END
	ELSE
	BEGIN
		SET @userCriterion = ' '
	END

	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Categories] c' +
	' WHERE ' + @catCriterion + ' c.visible = 0 ' + @userCriterion +
	' ORDER BY c.ID DESC'
	EXECUTE (@sql)
	--SELECT @sql
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastDeletedCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastDeletedCompanies]
	@nameContains NVARCHAR(100),
	@count BIGINT,
	@byUser BIGINT,
	@compID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @compCriterion NVARCHAR(150)
	DECLARE @userCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@nameContains IS NOT NULL AND @compID IS NOT NULL)
	BEGIN
		RAISERROR('nameContains is not null AND compID is not null', 16, 1)
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	IF(@nameContains IS NOT NULL)
	BEGIN
		SET @compCriterion = 'c.name LIKE ''%' + @nameContains + '%'' AND'
	END
	ELSE IF(@compID IS NOT NULL)
	BEGIN
		SET @compCriterion = 'c.ID = ' + CAST(@compID AS NVARCHAR) + ' AND'
	END
	ELSE
	BEGIN
		SET @compCriterion = ' '
	END

	IF(@byUser IS NOT NULL)
	BEGIN
		SET @userCriterion = ' AND c.lastModifiedBy = ' + CAST(@byUser AS NVARCHAR)
	END
	ELSE
	BEGIN
		SET @userCriterion = ' '
	END

	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Companies] c' +
	' WHERE ' + @compCriterion + ' c.visible = 0 ' + @userCriterion +
	' ORDER BY c.ID DESC'
	EXECUTE (@sql)
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastDeletedProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastDeletedProducts]
	@nameContains NVARCHAR(100),
	@count BIGINT,
	@byUser BIGINT,
	@prodID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @prodCriterion NVARCHAR(150)
	DECLARE @userCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@nameContains IS NOT NULL AND @prodID IS NOT NULL)
	BEGIN
		RAISERROR('nameContains is not null AND prodID is not null', 16, 1)
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	IF(@nameContains IS NOT NULL)
	BEGIN
		SET @prodCriterion = 'p.name LIKE ''%' + @nameContains + '%'' AND'
	END
	ELSE IF(@prodID IS NOT NULL)
	BEGIN
		SET @prodCriterion = 'p.ID = ' + CAST(@prodID AS NVARCHAR) + ' AND'
	END
	ELSE
	BEGIN
		SET @prodCriterion = ' '
	END

	IF(@byUser IS NOT NULL)
	BEGIN
		SET @userCriterion = ' AND p.lastModifiedBy = ' + CAST(@byUser AS NVARCHAR)
	END
	ELSE
	BEGIN
		SET @userCriterion = ' '
	END

	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Products] p' +
	' WHERE ' + @prodCriterion + ' p.visible = 0 ' + @userCriterion +
	' ORDER BY p.ID DESC'
	EXECUTE (@sql)
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastDeletedProductTopics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastDeletedProductTopics]
	@nameContains NVARCHAR(100),
	@count BIGINT,
	@byUser BIGINT,
	@topicID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @topicCriterion NVARCHAR(150)
	DECLARE @userCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@nameContains IS NOT NULL AND @topicID IS NOT NULL)
	BEGIN
		RAISERROR('nameContains is not null AND topicID is not null', 16, 1)
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	IF(@nameContains IS NOT NULL)
	BEGIN
		SET @topicCriterion = 'p.name LIKE ''%' + @nameContains + '%'' AND'
	END
	ELSE IF(@topicID IS NOT NULL)
	BEGIN
		SET @topicCriterion = 'p.ID = ' + CAST(@topicID AS NVARCHAR) + ' AND'
	END
	ELSE
	BEGIN
		SET @topicCriterion = ' '
	END

	IF(@byUser IS NOT NULL)
	BEGIN
		SET @userCriterion = ' AND p.LastModifiedBy = ' + CAST(@byUser AS NVARCHAR)
	END
	ELSE
	BEGIN
		SET @userCriterion = ' '
	END

	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[ProductTopics] p' +
	' WHERE ' + @topicCriterion + ' p.visible = 0 ' + @userCriterion +
	' ORDER BY p.ID DESC'
	EXECUTE (@sql)
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastProducts]
	@number BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Products] p
	ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLastTopics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastTopics]
	@number BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[ProductTopics] t
	ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetLogsWithIP]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLogsWithIP]
	@ipAdress NVARCHAR(100),
	@number BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	
	IF (@ipAdress IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Logs] l
	WHERE l.userIPadress = @ipAdress
	ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetMostCommentedProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetMostCommentedProductsInCategory]
	@number BIGINT,
	@catID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@number IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@number')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	SELECT TOP (@number) * FROM [dbo].[Products] p
	WHERE p.categoryID = @catID AND p.comments > 0 AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1
	ORDER BY comments DESC
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetProductsWithMostRecentComments]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProductsWithMostRecentComments] 
	@NumberOfProducts INT 
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@NumberOfProducts IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@NumberOfProducts')
		RETURN
	END

	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	SELECT TOP (@NumberOfProducts) * FROM [dbo].[Products] p, [dbo].[Comments] c
	WHERE [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 AND 
		c.ID = [dbo].[ProductLastCreatedCommentID](p.ID) AND
		p.ID = c.typeID
	ORDER BY c.lastModified DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[GetProductTopics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProductTopics]
	@prodID BIGINT,
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@sorted BIT
	
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@prodID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@prodID')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@sorted IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@sorted')
		RETURN
	END
	
	DECLARE @topicIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@sorted = 1)
	BEGIN
	
	DECLARE Topics_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[ProductTopics] 
	WHERE [Product] = @prodID AND [visible] = 1
	ORDER BY lastCommentDate DESC
	
	SET @index = 0
	OPEN Topics_Cursor
	FETCH NEXT FROM Topics_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @topicIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Topics_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Topics_Cursor
	DEALLOCATE Topics_Cursor
	
	END
	ELSE
	BEGIN
	
	DECLARE Topics_Cursor CURSOR FOR
	SELECT ID FROM [dbo].[ProductTopics] 
	WHERE [Product] = @prodID AND [visible] = 1
	ORDER BY ID ASC
	
	SET @index = 0
	OPEN Topics_Cursor
	FETCH NEXT FROM Topics_Cursor INTO @ID
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @topicIDs(cID) VALUES(@ID)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Topics_Cursor INTO @ID
		END
		SET @index = (@index + 1)
	END
	CLOSE Topics_Cursor
	DEALLOCATE Topics_Cursor
	
	END
	
	
	IF (@sorted = 1)
	BEGIN
		SELECT * FROM [dbo].[ProductTopics] WHERE ID IN (SELECT cID FROM @topicIDs) ORDER BY lastCommentDate DESC
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[ProductTopics] WHERE ID IN (SELECT cID FROM @topicIDs) ORDER BY ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[GetSuggestionsForCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSuggestionsForCategory]
	@category NVARCHAR(100),
	@fromIndex BIGINT,
	@toIndex BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@category IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@category')
		RETURN
	END
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	
	DECLARE @suggestionIDs TABLE(cID BIGINT NOT NULL PRIMARY KEY)
	DECLARE @ID BIGINT
	DECLARE @index BIGINT
	
	
	IF (@category = 'all' )
	BEGIN
	
		DECLARE Suggestions_Cursor CURSOR FOR
		SELECT ID FROM [dbo].[Suggestions] WHERE visible = 1
		ORDER BY ID DESC
		
		SET @index = 0
		OPEN Suggestions_Cursor
		FETCH NEXT FROM Suggestions_Cursor INTO @ID
		WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
		BEGIN
			IF ((@fromIndex <= @index) AND (@index < @toIndex))
			BEGIN
				INSERT INTO @suggestionIDs(cID) VALUES(@ID)
			END
			IF (@index < @toIndex)
			BEGIN
				FETCH NEXT FROM Suggestions_Cursor INTO @ID
			END
			SET @index = (@index + 1)
		END
		CLOSE Suggestions_Cursor
		DEALLOCATE Suggestions_Cursor
	
	END
	ELSE
	BEGIN
		
		DECLARE Suggestions_Cursor CURSOR FOR
		SELECT ID FROM [dbo].[Suggestions] WHERE [category] = @category AND visible = 1
		ORDER BY ID DESC
		
		SET @index = 0
		OPEN Suggestions_Cursor
		FETCH NEXT FROM Suggestions_Cursor INTO @ID
		WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
		BEGIN
			IF ((@fromIndex <= @index) AND (@index < @toIndex))
			BEGIN
				INSERT INTO @suggestionIDs(cID) VALUES(@ID)
			END
			IF (@index < @toIndex)
			BEGIN
				FETCH NEXT FROM Suggestions_Cursor INTO @ID
			END
			SET @index = (@index + 1)
		END
		CLOSE Suggestions_Cursor
		DEALLOCATE Suggestions_Cursor
	
	END
	
	SELECT * FROM [dbo].[Suggestions] WHERE ID IN (SELECT cID FROM @suggestionIDs) ORDER BY ID DESC
	
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindAlternativeCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindAlternativeCompanies]
	@searchKeywords NVARCHAR(3998),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localAlternativeCompanies TABLE(lapID BIGINT NOT NULL PRIMARY KEY, lapName NVARCHAR(100) NOT NULL, lapHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
		
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @likePattern NVARCHAR(4000)  -- >= (3998 + 2)
		
		INSERT INTO @localAlternativeCompanies(lapID, lapName, lapHits) 
			SELECT ID, name, 0 FROM [dbo].[AlternativeCompanyNames] 
			WHERE [visible] = 1 AND [dbo].[AreCompanyRelationsOk](Company) = 1

		SET @likePattern = ('%' + @searchKeywords + '%')
		UPDATE @localAlternativeCompanies SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

		SET @likePattern = (@searchKeywords + '%')
		UPDATE @localAlternativeCompanies SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

		UPDATE @localAlternativeCompanies SET lapHits = (lapHits + 1) WHERE lapName = @searchKeywords   
	END
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lapID, lapHits FROM @localAlternativeCompanies WHERE [lapHits] > 0 ORDER BY [lapHits] DESC, [lapName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(apn.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[AlternativeCompanyNames] apn, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND apn.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT acn.* FROM [dbo].[AlternativeCompanyNames] acn, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND acn.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, acn.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindAlternativeProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindAlternativeProducts]
	@searchKeywords NVARCHAR(3998),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localAlternativeProducts TABLE(lapID BIGINT NOT NULL PRIMARY KEY, lapName NVARCHAR(100) NOT NULL, lapHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @likePattern NVARCHAR(4000)  -- >= (3998 + 2)
		
		INSERT INTO @localAlternativeProducts(lapID, lapName, lapHits) 
			SELECT ID, name, 0 FROM [dbo].[AlternativeProductNames] 
			WHERE [visible] = 1 AND [dbo].[AreProductRelationsOk](Product, @otherCompanyID) = 1

		SET @likePattern = ('%' + @searchKeywords + '%')
		UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

		SET @likePattern = (@searchKeywords + '%')
		UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

		UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName = @searchKeywords
	    
	    
	END
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lapID, lapHits FROM @localAlternativeProducts WHERE [lapHits] > 0 ORDER BY [lapHits] DESC, [lapName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(apn.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[AlternativeProductNames] apn, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND apn.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT apn.* FROM [dbo].[AlternativeProductNames] apn, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND apn.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, apn.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindAlternativeProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindAlternativeProductsInCategory] 
	@searchKeywords NVARCHAR(3998),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localAlternativeProducts TABLE(lapID BIGINT NOT NULL PRIMARY KEY, lapName NVARCHAR(100) NOT NULL, lapHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @likePattern NVARCHAR(4000)  
			
			INSERT INTO @localAlternativeProducts(lapID, lapName, lapHits) 
				SELECT apn.ID, apn.name, 0 FROM [dbo].[AlternativeProductNames] apn, [dbo].[Products] p
				WHERE apn.[visible] = 1 AND p.[ID] = apn.[Product] AND p.[categoryID] = @catID AND 
					[dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 

				SET @likePattern = ('%' + @searchKeywords + '%')
				UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

			SET @likePattern = (@searchKeywords + '%')
			UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName LIKE @likePattern

			UPDATE @localAlternativeProducts SET lapHits = (lapHits + 1) WHERE lapName = @searchKeywords
		END
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lapID, lapHits FROM @localAlternativeProducts WHERE [lapHits] > 0 ORDER BY [lapHits] DESC, [lapName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT @fakeID AS ID, CAST(COUNT(apn.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[AlternativeProductNames] apn, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND apn.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT apn.* 
			FROM [dbo].[AlternativeProductNames] apn, @IdsAndHits ih 
			WHERE ih.[ihHits] > 0 AND apn.[ID] = ih.[ihID] 
			ORDER BY ih.[ihHits] DESC, apn.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindCategories] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localCategories TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localCategories(lcID, lcName, lcHits) SELECT ID, name, 0 FROM [dbo].[Categories] WHERE [visible] = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localCategories SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		UPDATE @localCategories SET lcHits = (lcHits + 1) WHERE lcName = @searchKeywords
		
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localCategories WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(c.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[Categories] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
	SELECT c.* FROM [dbo].[Categories] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, c.[name]
	END
END
GO
/****** Object:  StoredProcedure [dbo].[InternalFindCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindCompanies] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localCompanies TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
		IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localCompanies(lcID, lcName, lcHits) SELECT ID, name, 0 FROM [dbo].[Companies] WHERE [visible] = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localCompanies SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		UPDATE @localCompanies SET lcHits = (lcHits + 1) WHERE lcName = @searchKeywords
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localCompanies WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(c.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[Companies] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
	SELECT c.* FROM [dbo].[Companies] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, c.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindCompaniesWithType]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindCompaniesWithType] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@typeID BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localCompanies TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@typeID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@typeID')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localCompanies(lcID, lcName, lcHits) SELECT ID, name, 0 FROM [dbo].[Companies] WHERE [type] = @typeID AND [visible] = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localCompanies SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localCompanies WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(c.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[Companies] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
	SELECT c.* FROM [dbo].[Companies] c, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND c.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, c.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProducts]
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localProducts TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localProducts(lcID, lcName, lcHits) SELECT ID, name, 0 FROM [dbo].[Products] WHERE [dbo].[AreProductRelationsOk](ID, @otherCompanyID) = 1 --[visible] = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localProducts SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		UPDATE @localProducts SET lcHits = (lcHits + 1) WHERE lcName = @searchKeywords
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localProducts WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(p.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[Products] p, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND p.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT p.* FROM [dbo].[Products] p, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND p.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, p.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProductsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProductsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localProducts TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localProducts(lcID, lcName, lcHits) SELECT ID, name, 0 FROM [dbo].[Products] WHERE [categoryID] = @catID AND [dbo].[AreProductRelationsOk](ID, @otherCompanyID) = 1 

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localProducts SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		UPDATE @localProducts SET lcHits = (lcHits + 1) WHERE lcName = @searchKeywords
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localProducts WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(p.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[Products] p, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND p.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT p.* FROM [dbo].[Products] p, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND p.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, p.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProductSubVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProductSubVariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localSubVariants TABLE
	(
		lsvID BIGINT NOT NULL PRIMARY KEY, 
		lsvName NVARCHAR(100) NOT NULL, 
		lsvSubVariantHits BIGINT NOT NULL,
		lsvVariantHits BIGINT NOT NULL,
		lsvProductHits BIGINT NOT NULL
	)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localSubVariants(lsvID, lsvName, lsvSubVariantHits, lsvVariantHits, lsvProductHits) 
				SELECT psv.[ID], psv.[name], 0, 0, 0 
				FROM [dbo].[ProductSubVariants] psv, [dbo].[ProductVariants] pv, [dbo].[Products] p
				WHERE psv.[visible] = 1 AND 
					pv.[ID] = psv.[Variant] AND pv.[visible] = 1 AND
					p.[ID] = pv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 AND --[visible] = 1
					p.[ID] = psv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				
				-- SubVariant hits
				UPDATE @localSubVariants SET lsvSubVariantHits = (lsvSubVariantHits + 1) WHERE lsvName LIKE @likePattern
				
				-- Variant hits
				UPDATE @localSubVariants
					SET lsvVariantHits = (lsvVariantHits + 1) 
					WHERE [dbo].[ProductVariantNameOfProductSubVariantMatch](lsvID, @likePattern, 0) = 1

				-- Product hits
				UPDATE @localSubVariants
					SET lsvProductHits = (lsvProductHits + 1) 
					WHERE [dbo].[ProductNameOfProductSubVariantMatch](lsvID, @likePattern, 0) = 1

			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		-- SubVariant hits
		UPDATE @localSubVariants SET lsvSubVariantHits = (lsvSubVariantHits + 1) WHERE lsvName = @searchKeywords
				
		-- Variant hits
		UPDATE @localSubVariants
			SET lsvVariantHits = (lsvVariantHits + 1) 
			WHERE [dbo].[ProductVariantNameOfProductSubVariantMatch](lsvID, @searchKeywords, 1) = 1

		-- Product hits
		UPDATE @localSubVariants
			SET lsvProductHits = (lsvProductHits + 1) 
			WHERE [dbo].[ProductNameOfProductSubVariantMatch](lsvID, @searchKeywords, 1) = 1

	END
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
		SELECT lsvID, ([lsvSubVariantHits] + [lsvVariantHits] + [lsvProductHits])
		FROM @localSubVariants 
		WHERE ([lsvSubVariantHits] > 0) AND ([lsvProductHits] > 0)  
		ORDER BY ([lsvSubVariantHits] + [lsvVariantHits] + [lsvProductHits]) DESC, [lsvName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT @fakeID AS ID, CAST(COUNT(psv.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[ProductSubVariants] psv, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND psv.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT psv.* 
			FROM [dbo].[ProductSubVariants] psv, @IdsAndHits ih 
			WHERE ih.[ihHits] > 0 AND psv.[ID] = ih.[ihID] 
			ORDER BY ih.[ihHits] DESC, psv.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProductSubVariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProductSubVariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localSubVariants TABLE
	(
		lsvID BIGINT NOT NULL PRIMARY KEY, 
		lsvName NVARCHAR(100) NOT NULL, 
		lsvSubVariantHits BIGINT NOT NULL,
		lsvVariantHits BIGINT NOT NULL,
		lsvProductHits BIGINT NOT NULL
	)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localSubVariants(lsvID, lsvName, lsvSubVariantHits, lsvVariantHits, lsvProductHits) 
				SELECT psv.[ID], psv.[name], 0, 0, 0 
				FROM [dbo].[ProductSubVariants] psv, [dbo].[ProductVariants] pv, [dbo].[Products] p
				WHERE psv.[visible] = 1 AND 
					pv.[ID] = psv.[Variant] AND pv.[visible] = 1 AND
					p.[ID] = pv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 AND --[visible] = 1
					p.[ID] = psv.[Product] AND p.[categoryID] = @catID AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				
				-- SubVariant hits
				UPDATE @localSubVariants SET lsvSubVariantHits = (lsvSubVariantHits + 1) WHERE lsvName LIKE @likePattern
				
				-- Variant hits
				UPDATE @localSubVariants
					SET lsvVariantHits = (lsvVariantHits + 1) 
					WHERE [dbo].[ProductVariantNameOfProductSubVariantMatch](lsvID, @likePattern, 0) = 1
					
				-- Product hits
				UPDATE @localSubVariants
					SET lsvProductHits = (lsvProductHits + 1) 
					WHERE [dbo].[ProductNameOfProductSubVariantMatch](lsvID, @likePattern, 0) = 1

			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		-- SubVariant hits
		UPDATE @localSubVariants SET lsvSubVariantHits = (lsvSubVariantHits + 1) WHERE lsvName = @searchKeywords
				
		-- Variant hits
		UPDATE @localSubVariants
			SET lsvVariantHits = (lsvVariantHits + 1) 
			WHERE [dbo].[ProductVariantNameOfProductSubVariantMatch](lsvID, @searchKeywords, 1) = 1
				
		-- Product hits
		UPDATE @localSubVariants
			SET lsvProductHits = (lsvProductHits + 1) 
			WHERE [dbo].[ProductNameOfProductSubVariantMatch](lsvID, @searchKeywords, 1) = 1

	END
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
		SELECT lsvID, ([lsvSubVariantHits] + [lsvVariantHits] + [lsvProductHits])
		FROM @localSubVariants 
		WHERE ([lsvSubVariantHits] > 0) AND ([lsvProductHits] > 0)  
		ORDER BY ([lsvSubVariantHits] + [lsvVariantHits] + [lsvProductHits]) DESC, [lsvName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT @fakeID AS ID, CAST(COUNT(psv.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[ProductSubVariants] psv, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND psv.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT psv.* 
			FROM [dbo].[ProductSubVariants] psv, @IdsAndHits ih 
			WHERE ih.[ihHits] > 0 AND psv.[ID] = ih.[ihID] 
			ORDER BY ih.[ihHits] DESC, psv.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProductVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProductVariants] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localVariants TABLE
	(
		lvID BIGINT NOT NULL PRIMARY KEY, 
		lvName NVARCHAR(100) NOT NULL, 
		lvVariantHits BIGINT NOT NULL,
		lvProductHits BIGINT NOT NULL
	)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localVariants(lvID, lvName, lvVariantHits, lvProductHits) 
				SELECT pv.[ID], pv.[name], 0, 0 
				FROM [dbo].[ProductVariants] pv, [dbo].[Products] p
				WHERE pv.[visible] = 1 AND 
					p.[ID] = pv.[Product] AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				
				-- Variant hits
				UPDATE @localVariants SET lvVariantHits = (lvVariantHits + 1) WHERE lvName LIKE @likePattern
				
				-- Product hits
				UPDATE @localVariants
					SET lvProductHits = (lvProductHits + 1) 
					WHERE [dbo].[ProductNameOfProductVariantMatch](lvID, @likePattern, 0) = 1

			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		-- Variant hits
		UPDATE @localVariants SET lvVariantHits = (lvVariantHits + 1) WHERE lvName = @searchKeywords
				
		-- Product hits
		UPDATE @localVariants
			SET lvProductHits = (lvProductHits + 1) 
			WHERE [dbo].[ProductNameOfProductVariantMatch](lvID, @searchKeywords, 1) = 1

	END
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
		SELECT lvID, ([lvVariantHits] + [lvProductHits])
		FROM @localVariants 
		WHERE ([lvVariantHits] > 0) AND ([lvProductHits] > 0) 
		ORDER BY ([lvVariantHits] + [lvProductHits]) DESC, [lvName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT @fakeID AS ID, CAST(COUNT(pv.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[ProductVariants] pv, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND pv.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT pv.* 
			FROM [dbo].[ProductVariants] pv, @IdsAndHits ih 
			WHERE ih.[ihHits] > 0 AND pv.[ID] = ih.[ihID] 
			ORDER BY ih.[ihHits] DESC, pv.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindProductVariantsInCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindProductVariantsInCategory] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@catID BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localVariants TABLE
	(
		lvID BIGINT NOT NULL PRIMARY KEY, 
		lvName NVARCHAR(100) NOT NULL, 
		lvVariantHits BIGINT NOT NULL,
		lvProductHits BIGINT NOT NULL
	)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	IF (@catID IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@catID')
		RETURN
	END
	
	DECLARE @otherCompanyID BIGINT
	
	SET @otherCompanyID = [dbo].[GetOtherCompanyID]()
	IF (@otherCompanyID IS NULL)
	BEGIN
		RAISERROR('No ''other'' company or no ''system'' user.', 16, 1)
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localVariants(lvID, lvName, lvVariantHits, lvProductHits) 
				SELECT pv.[ID], pv.[name], 0, 0 
				FROM [dbo].[ProductVariants] pv, [dbo].[Products] p
				WHERE pv.[visible] = 1 AND 
					p.[ID] = pv.[Product] AND p.[categoryID] = @catID AND [dbo].[AreProductRelationsOk](p.[ID], @otherCompanyID) = 1 

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				
				-- Variant hits
				UPDATE @localVariants SET lvVariantHits = (lvVariantHits + 1) WHERE lvName LIKE @likePattern
				
				-- Product hits
				UPDATE @localVariants
					SET lvProductHits = (lvProductHits + 1) 
					WHERE [dbo].[ProductNameOfProductVariantMatch](lvID, @likePattern, 0) = 1

			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
		
		-- Variant hits
		UPDATE @localVariants SET lvVariantHits = (lvVariantHits + 1) WHERE lvName = @searchKeywords
				
		-- Product hits
		UPDATE @localVariants
			SET lvProductHits = (lvProductHits + 1) 
			WHERE [dbo].[ProductNameOfProductVariantMatch](lvID, @searchKeywords, 1) = 1

	END
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
		SELECT lvID, ([lvVariantHits] + [lvProductHits])
		FROM @localVariants 
		WHERE ([lvVariantHits] > 0) AND ([lvProductHits] > 0) 
		ORDER BY ([lvVariantHits] + [lvProductHits]) DESC, [lvName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT @fakeID AS ID, CAST(COUNT(pv.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM [dbo].[ProductVariants] pv, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND pv.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
		SELECT pv.* 
			FROM [dbo].[ProductVariants] pv, @IdsAndHits ih 
			WHERE ih.[ihHits] > 0 AND pv.[ID] = ih.[ihID] 
			ORDER BY ih.[ihHits] DESC, pv.[name]
	END
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindUsers]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InternalFindUsers] 
	@searchKeywords NVARCHAR(3999),
	@fromIndex BIGINT,
	@toIndex BIGINT,
	@countOnly BIT
AS
BEGIN
	DECLARE @localUsers TABLE(lcID BIGINT NOT NULL PRIMARY KEY, lcName NVARCHAR(100) NOT NULL, lcHits BIGINT NOT NULL)
	
	DECLARE @errMsgFmt VARCHAR(200)
	SET @errMsgFmt = '%s parameter must not be NULL.'
	IF (@fromIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@fromIndex')
		RETURN
	END
	IF (@toIndex IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@toIndex')
		RETURN
	END
	IF (@countOnly IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@countOnly')
		RETURN
	END
	
	IF (ISNULL(@searchKeywords, '') != '')
	BEGIN
		DECLARE @keywordsTbl TABLE(kID INT NOT NULL PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
		
		INSERT INTO @keywordsTbl SELECT * FROM [dbo].[StringSplit](@searchKeywords, ' ')
		
		IF((SELECT COUNT(kID) FROM @keywordsTbl) > 0)
		BEGIN
			DECLARE @keyword NVARCHAR(500)
			DECLARE @likePattern NVARCHAR(510)  -- >= (500 + 2)
			DECLARE Keyword_cursor CURSOR FOR SELECT kKeyword FROM @keywordsTbl
			
			INSERT INTO @localUsers(lcID, lcName, lcHits) SELECT ID, username, 0 FROM wiadvice_userDB.[dbo].[Users] WHERE [visible] = 1 AND ( [type] = 'user' OR [type] = 'writer')

			OPEN Keyword_cursor
		    FETCH Keyword_cursor INTO @keyword
		    WHILE (@@FETCH_STATUS = 0)
		    BEGIN
				SET @likePattern = ('%' + @keyword + '%')
				UPDATE @localUsers SET lcHits = (lcHits + 1) WHERE lcName LIKE @likePattern
			    FETCH Keyword_cursor INTO @keyword
		    END
			CLOSE Keyword_cursor
			DEALLOCATE Keyword_cursor
		    
		END
	END
	
	
	
	DECLARE @IdsAndHits TABLE(ihID BIGINT NOT NULL PRIMARY KEY, ihHits INT NOT NULL)
	DECLARE @hits INT
	DECLARE @ID INT
	DECLARE @index BIGINT
	
	DECLARE Hits_Cursor CURSOR FOR
	SELECT lcID, lcHits FROM @localUsers WHERE [lcHits] > 0 ORDER BY [lcHits] DESC, [lcName]
	
	SET @index = 0
	OPEN Hits_Cursor
	FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
	WHILE ((@@FETCH_STATUS = 0) AND(@index < @toIndex))
	BEGIN
		IF ((@fromIndex <= @index) AND (@index < @toIndex))
		BEGIN
			INSERT INTO @IdsAndHits(ihID, ihHits) VALUES(@ID, @hits)
		END
		IF (@index < @toIndex)
		BEGIN
			FETCH NEXT FROM Hits_Cursor INTO @ID, @hits 
		END
		SET @index = (@index + 1)
	END
	CLOSE Hits_Cursor
	DEALLOCATE Hits_Cursor
	
	IF(@countOnly = 1)
	BEGIN
		DECLARE @fakeID BIGINT
		SET @fakeID = 1
		SELECT 
			@fakeID AS ID, CAST(COUNT(u.ID) AS NVARCHAR(3500)) AS Value, 'bigint' AS DataType
			FROM wiadvice_userDB.[dbo].[Users] u, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND u.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
	SELECT u.* FROM wiadvice_userDB.[dbo].[Users] u, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND u.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, u.[username]
	END
END
GO
/****** Object:  UserDefinedFunction [dbo].[AreCompanyRelationsOk]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[AreCompanyRelationsOk] 
(
	@companyID BIGINT
)
RETURNS BIT
AS
BEGIN
	DECLARE @result BIT

	SET @result = 0
	IF(@companyID IS NOT NULL)
	BEGIN
		IF
		(
			(SELECT COUNT(ID) FROM [dbo].[Companies] c
				WHERE c.ID = @companyID AND c.[visible] = 1 ) = 1
		)
		BEGIN
			SET @result = 1
		END
	END
	RETURN @result
END

GO
/****** Object:  UserDefinedFunction [dbo].[AreProductRelationsOk]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[AreProductRelationsOk] 
(
	@productID BIGINT, 
	@otherCompanyID BIGINT
)
RETURNS BIT
AS
BEGIN
	DECLARE @result BIT

	SET @result = 0
	IF((@productID IS NOT NULL) AND (@otherCompanyID IS NOT NULL))
	BEGIN
		IF
		(
			(SELECT COUNT(ID) FROM [dbo].[Products] p
				WHERE p.ID = @productID AND p.[visible] = 1 AND 
				(
					(p.[companyID] = @otherCompanyID)
					OR
					(p.[companyID] != @otherCompanyID AND
					((SELECT COUNT(ID) FROM [dbo].[Categories_Companies] cc WHERE cc.[visible] = 1 AND 
						cc.[companyID] = p.[companyID] AND cc.[categoryID] = p.[categoryID]) > 0))
				)
				AND
				((SELECT COUNT(ID) FROM [dbo].[Companies] c WHERE p.[companyID] = c.[ID] AND c.[visible] = 1) > 0) AND
				((SELECT COUNT(ID) FROM [dbo].[Categories] cat WHERE p.[categoryID] = cat.[ID] AND cat.[visible] = 1) > 0)
			) 
			= 1
		)
		BEGIN
			SET @result = 1
		END
	END
	RETURN @result
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetCommentRating]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetCommentRating] 
(
	@commentID BIGINT
)
RETURNS INT
AS
BEGIN
	DECLARE @result INT

	SET @result = 0
	IF(@commentID IS NOT NULL)
	BEGIN
		SET @result = (SELECT (c.agrees - c.disagrees) 
		FROM [dbo].[Comments] c
		WHERE c.[ID] = @commentID)		
	END
	SET @result = ISNULL(@result, 0)
	RETURN @result
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetOtherCompanyID]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================

CREATE FUNCTION [dbo].[GetOtherCompanyID] 
(
)
RETURNS BIGINT
AS
BEGIN
	DECLARE @systemUserID BIGINT
	DECLARE @otherCompanyID BIGINT

	SET @otherCompanyID = NULL
	SET @systemUserID = 
		(SELECT ID FROM wiadvice_userDB.[dbo].[Users] WHERE [username] = 'system' AND [type] = 'system')

	IF(@systemUserID IS NOT NULL)
	BEGIN
		SET @otherCompanyID = 
			(SELECT ID FROM [dbo].[Companies] 
			WHERE [name] = 'other' AND [createdBy] = @systemUserID)
	END

	RETURN @otherCompanyID
END

GO
/****** Object:  UserDefinedFunction [dbo].[ProductLastCreatedCommentID]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ProductLastCreatedCommentID]
(
	@ProductID BIGINT
)
RETURNS BIGINT
AS
BEGIN
	DECLARE @lastModifiedCommentID BIGINT
	
	SET @lastModifiedCommentID = NULL

	IF (@ProductID IS NOT NULL)
	BEGIN
		SET @lastModifiedCommentID = 
			(SELECT TOP 1 c.ID FROM [dbo].[Comments] c, [dbo].[Products] p
			WHERE c.[type] = 'product' AND c.visible = 1 AND c.typeID = @ProductID
			ORDER BY c.dateCreated DESC, c.ID DESC)
	END
	
	RETURN @lastModifiedCommentID

END

GO
/****** Object:  UserDefinedFunction [dbo].[ProductNameOfProductSubVariantMatch]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ProductNameOfProductSubVariantMatch] 
(
	@productSubVariantID BIGINT,
	@keyword NVARCHAR(3999),
	@exact BIT
)
RETURNS BIT
AS
BEGIN
	DECLARE @match BIT

	SET @match = 0
	
	IF ((ISNULL(@productSubVariantID, 0) > 0) AND (ISNULL(@keyword, '') != ''))
	BEGIN
		IF (ISNULL(@exact, 1) = 1)
		BEGIN
			IF
			(
				(SELECT COUNT(p.[ID])
					FROM [dbo].[ProductSubVariants] psv, [dbo].[Products] p
					WHERE psv.[ID] = @productSubVariantID AND
						p.[ID] = psv.[Product] AND
						p.[name] = @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
		ELSE
		BEGIN
			IF
			(
				(SELECT COUNT(p.[ID])
					FROM [dbo].[ProductSubVariants] psv, [dbo].[Products] p
					WHERE psv.[ID] = @productSubVariantID AND
						p.[ID] = psv.[Product] AND
						p.[name] LIKE @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
	END

	RETURN @match

END

GO
/****** Object:  UserDefinedFunction [dbo].[ProductNameOfProductVariantMatch]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ProductNameOfProductVariantMatch] 
(
	@productVariantID BIGINT,
	@keyword NVARCHAR(3999),
	@exact BIT
)
RETURNS BIT
AS
BEGIN
	DECLARE @match BIT

	SET @match = 0
	
	IF ((ISNULL(@productVariantID, 0) > 0) AND (ISNULL(@keyword, '') != ''))
	BEGIN
		IF (ISNULL(@exact, 1) = 1)
		BEGIN
			IF
			(
				(SELECT COUNT(p.[ID])
					FROM [dbo].[ProductVariants] pv, [dbo].[Products] p
					WHERE pv.[ID] = @productVariantID AND
						p.[ID] = pv.[Product] AND
						p.[name] = @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
		ELSE
		BEGIN
			IF
			(
				(SELECT COUNT(p.[ID])
					FROM [dbo].[ProductVariants] pv, [dbo].[Products] p
					WHERE pv.[ID] = @productVariantID AND
						p.[ID] = pv.[Product] AND
						p.[name] LIKE @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
	END

	RETURN @match

END

GO
/****** Object:  UserDefinedFunction [dbo].[ProductVariantNameOfProductSubVariantMatch]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ProductVariantNameOfProductSubVariantMatch] 
(
	@productSubVariantID BIGINT,
	@keyword NVARCHAR(3999),
	@exact BIT
)
RETURNS BIT
AS
BEGIN
	DECLARE @match BIT

	SET @match = 0
	
	IF ((ISNULL(@productSubVariantID, 0) > 0) AND (ISNULL(@keyword, '') != ''))
	BEGIN
		IF (ISNULL(@exact, 1) = 1)
		BEGIN
			IF
			(
				(SELECT COUNT(pv.[ID])
					FROM [dbo].[ProductSubVariants] psv, [dbo].[ProductVariants] pv
					WHERE psv.[ID] = @productSubVariantID AND
						pv.[ID] = psv.[Variant] AND
						pv.[name] = @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
		ELSE
		BEGIN
			IF
			(
				(SELECT COUNT(pv.[ID])
					FROM [dbo].[ProductSubVariants] psv, [dbo].[ProductVariants] pv
					WHERE psv.[ID] = @productSubVariantID AND
						pv.[ID] = psv.[Variant] AND
						pv.[name] LIKE @keyword
				) > 0
			)
			BEGIN
				SET @match = 1
			END
		END
	END

	RETURN @match

END

GO
/****** Object:  UserDefinedFunction [dbo].[StringSplit]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[StringSplit] 
(
	@string NVARCHAR(3999), 
	@delimiter NCHAR
)
RETURNS 
@keywordsTbl TABLE(kID INT NOT NULL IDENTITY PRIMARY KEY, kKeyword NVARCHAR(500) NOT NULL)
AS
BEGIN
	IF (ISNULL(@string, '') != '')
	BEGIN
		DECLARE @delimiterPosition INT
		DECLARE @startPosition INT
		DECLARE @stringLen INT
		DECLARE @keyword NVARCHAR(500)
		DECLARE @localDelimiter NCHAR  -- Make a copy in order not to propagate the changes to the caller
		DECLARE @localString NVARCHAR(4000)  -- (3999 + 1) for trailing delimiter if necessary. Make a copy in order not to propagate the changes to the caller
		
		SET @localDelimiter = ISNULL(@delimiter, ' ')
		IF(@localDelimiter = ' ')
		BEGIN
			SET @stringLen = LEN(@string)  -- LEN does not count the trailing spaces!
			SET @localString = SUBSTRING(@string, 1, @stringLen)  -- Trim the trailing spaces
		END
		ELSE
		BEGIN
			SET @localString = (@string + @localDelimiter)
			SET @stringLen = LEN(@localString)
		END
		SET @startPosition = 1
		SET @delimiterPosition = CHARINDEX(@localDelimiter, @localString, @startPosition)
		IF(@delimiterPosition > 0)
		BEGIN
			WHILE ((@delimiterPosition > 0) AND (@delimiterPosition <= @stringLen))
			BEGIN
				SET @keyword = SUBSTRING(@localString, @startPosition, (@delimiterPosition + 1 - @startPosition - 1))  -- "- 1" here because we do not want the delimiter after the word
				IF((LEN(@keyword) > 0) AND ((SELECT COUNT(kID) FROM @keywordsTbl WHERE kKeyword = @keyword) = 0))
				BEGIN
					INSERT INTO @keywordsTbl(kKeyword) VALUES(@keyword)
				END
				SET @startPosition = (@delimiterPosition + 1)
				IF(@delimiterPosition <= @stringLen)
				BEGIN
					SET @delimiterPosition = CHARINDEX(@localDelimiter, @localString, @startPosition)
					IF(@delimiterPosition = 0)
					BEGIN
						SET @keyword = SUBSTRING(@localString, @startPosition, (@stringLen + 1 - @startPosition))
						IF((LEN(@keyword) > 0) AND ((SELECT COUNT(kID) FROM @keywordsTbl WHERE kKeyword = @keyword) = 0))
						BEGIN
							INSERT INTO @keywordsTbl(kKeyword) VALUES(@keyword)
						END
					END
				END
			END
		END
		ELSE
		BEGIN
			INSERT INTO @keywordsTbl(kKeyword) VALUES(@localString)
		END
	END
	
	RETURN 
END

GO
/****** Object:  Table [dbo].[Advertisements]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Advertisements](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[html] [ntext] NULL,
	[info] [ntext] NULL,
	[adpath] [nvarchar](520) NULL,
	[adurl] [nvarchar](520) NULL,
	[general] [bit] NOT NULL,
	[expireDate] [datetime] NULL,
	[visible] [bit] NOT NULL,
	[active] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[targetUrl] [ntext] NOT NULL,
 CONSTRAINT [PK_Advertisements] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdvertisementsForCategories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertisementsForCategories](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Advertisement] [bigint] NOT NULL,
	[Category] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_AdvertisementsForCategories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdvertisementsForCompanies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertisementsForCompanies](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Advertisement] [bigint] NOT NULL,
	[Company] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_AdvertisementsForCompanies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdvertisementsForProducts]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertisementsForProducts](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Advertisement] [bigint] NOT NULL,
	[Product] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AlternativeCompanyNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlternativeCompanyNames](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Company] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_AlternativeCompanyNames] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AlternativeProductNames]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlternativeProductNames](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_AlternativeProductNames] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AutoCompleteSearch]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AutoCompleteSearch](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[string] [nvarchar](max) NOT NULL,
	[searches] [bigint] NOT NULL,
	[foundCompanies] [bigint] NOT NULL,
	[foundProducts] [bigint] NOT NULL,
	[foundCategories] [bigint] NOT NULL,
	[foundUsers] [bigint] NOT NULL,
	[foundResults] [bigint] NOT NULL,
	[dateFirstSearched] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoCompleteSearch] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[parentID] [bigint] NULL,
	[name] [nvarchar](100) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[last] [bit] NOT NULL,
	[description] [nvarchar](1000) NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
	[imageUrl] [nvarchar](520) NULL,
	[imageWidth] [int] NULL,
	[imageHeight] [int] NULL,
	[displayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories_Companies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories_Companies](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[categoryID] [bigint] NOT NULL,
	[companyID] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_Categories_Companies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommentRatings]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentRatings](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[User] [bigint] NOT NULL,
	[Comment] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[rating] [int] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_CommentRatings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Comments]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[typeID] [bigint] NOT NULL,
	[userID] [bigint] NOT NULL,
	[characteristicID] [bigint] NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [ntext] NULL,
	[visible] [bit] NOT NULL,
	[guestname] [nvarchar](50) NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[haveSubcomments] [bit] NOT NULL,
	[agrees] [bigint] NOT NULL,
	[disagrees] [bigint] NOT NULL,
	[ipAdress] [nvarchar](100) NOT NULL,
	[subType] [nvarchar](100) NOT NULL,
	[subTypeID] [bigint] NOT NULL,
	[Variant] [bigint] NULL,
	[SubVariant] [bigint] NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Companies]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [ntext] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[website] [nvarchar](1000) NULL,
	[canUserTakeRoleIfNoEditors] [bit] NOT NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompaniesCharacterestics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompaniesCharacterestics](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[companyID] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[description] [ntext] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_CompaniesCharacterestics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyImages]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyImages](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[compID] [bigint] NOT NULL,
	[url] [nvarchar](520) NOT NULL,
	[isLogo] [bit] NOT NULL,
	[description] [nvarchar](520) NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[isThumbnail] [bit] NOT NULL,
	[mainImgID] [bigint] NULL,
	[width] [int] NOT NULL,
	[height] [int] NOT NULL,
 CONSTRAINT [PK_CompanyImages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyTypes]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyTypes](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[visible] [bit] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK_CompanyTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Logs]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userID] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [ntext] NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[typeModifiedSubject] [nvarchar](100) NOT NULL,
	[IDModifiedSubject] [bigint] NOT NULL,
	[userIPadress] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NotifyOnNewContent]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotifyOnNewContent](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[typeID] [bigint] NOT NULL,
	[notifyUser] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[modifiedBy] [bigint] NOT NULL,
	[active] [bit] NOT NULL,
	[newInformation] [bit] NOT NULL,
 CONSTRAINT [PK_NotifyOnNewContent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductCharacteristics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCharacteristics](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[productID] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[description] [ntext] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[comments] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductCharacteristics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductImages]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImages](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[prodID] [bigint] NOT NULL,
	[main] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[description] [nvarchar](520) NULL,
	[url] [nvarchar](520) NOT NULL,
	[width] [int] NOT NULL,
	[height] [int] NOT NULL,
	[isThumbnail] [bit] NOT NULL,
	[mainImgID] [bigint] NULL,
 CONSTRAINT [PK_ProductImages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductLinks]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductLinks](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[link] [ntext] NOT NULL,
	[description] [ntext] NOT NULL,
	[User] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[dateLastModified] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_ProductLinks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductRatings]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductRatings](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[user] [bigint] NOT NULL,
	[Product] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[rating] [int] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_Ratings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[categoryID] [bigint] NOT NULL,
	[companyID] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[description] [ntext] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[rating] [bigint] NOT NULL,
	[usersRated] [bigint] NOT NULL,
	[comments] [bigint] NOT NULL,
	[website] [nvarchar](1000) NULL,
	[canUserTakeRoleIfNoEditors] [bit] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductsInUnspecifiedCategory]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductsInUnspecifiedCategory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[Category] [bigint] NOT NULL,
	[WantedCategory] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductsInUnspecifiedCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductSubVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSubVariants](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[Variant] [bigint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [nvarchar](max) NULL,
	[lastModified] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[comments] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductSubVariants] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductTopics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductTopics](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[comments] [bigint] NOT NULL,
	[visits] [bigint] NOT NULL,
	[User] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[lastCommentDate] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
	[locked] [bit] NOT NULL,
	[lastCommentBy] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductTopics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductVariants]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariants](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [bigint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](max) NULL,
	[lastModified] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[comments] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductVariants] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportComments]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportComments](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[user] [bigint] NOT NULL,
	[report] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[visible] [bit] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_ReportComments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Reports]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[description] [ntext] NULL,
	[reportType] [nvarchar](100) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[isResolved] [bit] NOT NULL,
	[isViewed] [bit] NOT NULL,
	[aboutType] [nvarchar](100) NOT NULL,
	[aboutTypeId] [bigint] NOT NULL,
	[aboutTypeParentId] [bigint] NULL,
	[isDeletedByUser] [bit] NOT NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ScalarValues]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ScalarValues](
	[ID] [bigint] NOT NULL,
	[Value] [nvarchar](3500) NOT NULL,
	[DataType] [varchar](500) NOT NULL,
 CONSTRAINT [PK_ScalarValues] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SiteNews]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteNews](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](400) NOT NULL,
	[description] [ntext] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[type] [nvarchar](400) NOT NULL,
	[visible] [bit] NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[linkID] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SiteText] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Statistics]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Statistics](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[forDate] [datetime] NOT NULL,
	[sessionsStarted] [bigint] NOT NULL,
	[usersLogged] [bigint] NOT NULL,
	[usersLoggedOut] [bigint] NOT NULL,
	[usersRegistered] [bigint] NOT NULL,
	[companiesCreated] [bigint] NOT NULL,
	[productsCreated] [bigint] NOT NULL,
	[commentsWritten] [bigint] NOT NULL,
	[reportsWritten] [bigint] NOT NULL,
	[picturesUploaded] [bigint] NOT NULL,
	[picturesDeleted] [bigint] NOT NULL,
	[companiesDeleted] [bigint] NOT NULL,
	[productsDeleted] [bigint] NOT NULL,
	[categoriesCreated] [bigint] NOT NULL,
	[categoriesDeleted] [bigint] NOT NULL,
	[usersDeleted] [bigint] NOT NULL,
	[adminsRegistered] [bigint] NOT NULL,
	[adminsDeleted] [bigint] NOT NULL,
	[prodCharsCreated] [bigint] NOT NULL,
	[compCharsCreated] [bigint] NOT NULL,
	[prodCharsDeleted] [bigint] NOT NULL,
	[compCharsDeleted] [bigint] NOT NULL,
	[commentsDeleted] [bigint] NOT NULL,
	[topicsCreated] [bigint] NOT NULL,
	[topicsDeleted] [bigint] NOT NULL,
 CONSTRAINT [PK_Statistics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Suggestions]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suggestions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[user] [bigint] NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[visible] [bit] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[lastModifiedBy] [bigint] NOT NULL,
	[category] [nvarchar](100) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_Suggestions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TransferTypeActions]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferTypeActions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserTransfering] [bigint] NOT NULL,
	[UserReceiving] [bigint] NOT NULL,
	[TypeAction] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[active] [bit] NOT NULL,
	[accepted] [bit] NOT NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransferTypeActions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeActions]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeActions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[typeID] [bigint] NOT NULL,
	[description] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_TypeActions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeSuggestionComments]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeSuggestionComments](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Suggestion] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[description] [ntext] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[User] [bigint] NOT NULL,
 CONSTRAINT [PK_TypeSuggestionComments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeSuggestions]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeSuggestions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[typeID] [bigint] NOT NULL,
	[active] [bit] NOT NULL,
	[visible] [bit] NOT NULL,
	[status] [nvarchar](100) NULL,
	[ByUser] [bigint] NOT NULL,
	[ToUser] [bigint] NOT NULL,
	[visibleByUser] [bit] NOT NULL,
	[visibleToUser] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[StatusBy] [bigint] NULL,
	[changedStatus] [datetime] NULL,
 CONSTRAINT [PK_TypeSuggestions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeWarnings]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeWarnings](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[User] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
	[UserTypeAction] [bigint] NOT NULL,
	[description] [ntext] NOT NULL,
	[ByAdmin] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NOT NULL,
	[modifiedReason] [ntext] NULL,
 CONSTRAINT [PK_TypeWarnings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserIDs]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserIDs](
	[ID] [bigint] NOT NULL,
	[haveNewContent] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersTypeActions]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersTypeActions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userID] [bigint] NOT NULL,
	[actionID] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[approvedBy] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_UsersTypeActions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Visits]    Script Date: 24.1.2015 г. 17:27:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Visits](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](200) NOT NULL,
	[typeID] [bigint] NOT NULL,
	[User] [bigint] NOT NULL,
	[dateVisited] [datetime] NOT NULL,
	[ipAdress] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Visits] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Idx_U_SiteText_Name]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Idx_U_SiteText_Name] ON [dbo].[SiteNews]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_last]  DEFAULT ((0)) FOR [last]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Categories_Companies] ADD  CONSTRAINT [DF_Categories_Companies_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF_Comments_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF_Comments_agrees]  DEFAULT ((0)) FOR [agrees]
GO
ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [DF_Comments_disagrees]  DEFAULT ((0)) FOR [disagrees]
GO
ALTER TABLE [dbo].[Companies] ADD  CONSTRAINT [DF_Companies_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[CompaniesCharacterestics] ADD  CONSTRAINT [DF_CompaniesCharacterestics_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[ProductCharacteristics] ADD  CONSTRAINT [DF_ProductCharacteristics_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_usersRated]  DEFAULT ((0)) FOR [usersRated]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_sessionsStarted]  DEFAULT ((0)) FOR [sessionsStarted]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_usersLogged]  DEFAULT ((0)) FOR [usersLogged]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_usersLoggedOut]  DEFAULT ((0)) FOR [usersLoggedOut]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_usersRegistered]  DEFAULT ((0)) FOR [usersRegistered]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_companiesCreated]  DEFAULT ((0)) FOR [companiesCreated]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_productsCreated]  DEFAULT ((0)) FOR [productsCreated]
GO
ALTER TABLE [dbo].[Statistics] ADD  CONSTRAINT [DF_Statistics_commentsWritten]  DEFAULT ((0)) FOR [commentsWritten]
GO
ALTER TABLE [dbo].[Advertisements]  WITH CHECK ADD  CONSTRAINT [FK_Advertisements_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Advertisements] CHECK CONSTRAINT [FK_Advertisements_CreatedBy]
GO
ALTER TABLE [dbo].[Advertisements]  WITH CHECK ADD  CONSTRAINT [FK_Advertisements_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Advertisements] CHECK CONSTRAINT [FK_Advertisements_LastModifiedBy]
GO
ALTER TABLE [dbo].[AdvertisementsForCategories]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForCategories_Advertisement] FOREIGN KEY([Advertisement])
REFERENCES [dbo].[Advertisements] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForCategories] CHECK CONSTRAINT [FK_AdvertisementsForCategories_Advertisement]
GO
ALTER TABLE [dbo].[AdvertisementsForCategories]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForCategories_Category] FOREIGN KEY([Category])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForCategories] CHECK CONSTRAINT [FK_AdvertisementsForCategories_Category]
GO
ALTER TABLE [dbo].[AdvertisementsForCompanies]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForCompanies_Advertisement] FOREIGN KEY([Advertisement])
REFERENCES [dbo].[Advertisements] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForCompanies] CHECK CONSTRAINT [FK_AdvertisementsForCompanies_Advertisement]
GO
ALTER TABLE [dbo].[AdvertisementsForCompanies]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForCompanies_Company] FOREIGN KEY([Company])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForCompanies] CHECK CONSTRAINT [FK_AdvertisementsForCompanies_Company]
GO
ALTER TABLE [dbo].[AdvertisementsForProducts]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForProducts_Advertisement] FOREIGN KEY([Advertisement])
REFERENCES [dbo].[Advertisements] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForProducts] CHECK CONSTRAINT [FK_AdvertisementsForProducts_Advertisement]
GO
ALTER TABLE [dbo].[AdvertisementsForProducts]  WITH CHECK ADD  CONSTRAINT [FK_AdvertisementsForProducts_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[AdvertisementsForProducts] CHECK CONSTRAINT [FK_AdvertisementsForProducts_Product]
GO
ALTER TABLE [dbo].[AlternativeCompanyNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeCompanyNames_Company] FOREIGN KEY([Company])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[AlternativeCompanyNames] CHECK CONSTRAINT [FK_AlternativeCompanyNames_Company]
GO
ALTER TABLE [dbo].[AlternativeCompanyNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeCompanyNames_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[AlternativeCompanyNames] CHECK CONSTRAINT [FK_AlternativeCompanyNames_CreatedBy]
GO
ALTER TABLE [dbo].[AlternativeCompanyNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeCompanyNames_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[AlternativeCompanyNames] CHECK CONSTRAINT [FK_AlternativeCompanyNames_LastModifiedBy]
GO
ALTER TABLE [dbo].[AlternativeProductNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeProductNames_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[AlternativeProductNames] CHECK CONSTRAINT [FK_AlternativeProductNames_CreatedBy]
GO
ALTER TABLE [dbo].[AlternativeProductNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeProductNames_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[AlternativeProductNames] CHECK CONSTRAINT [FK_AlternativeProductNames_LastModifiedBy]
GO
ALTER TABLE [dbo].[AlternativeProductNames]  WITH CHECK ADD  CONSTRAINT [FK_AlternativeProductNames_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[AlternativeProductNames] CHECK CONSTRAINT [FK_AlternativeProductNames_Product]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Users_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Users_CreatedBy]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Users_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Users_LastModifiedBy]
GO
ALTER TABLE [dbo].[Categories_Companies]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Companies] FOREIGN KEY([categoryID])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[Categories_Companies] CHECK CONSTRAINT [FK_Categories_Companies]
GO
ALTER TABLE [dbo].[Categories_Companies]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Companies_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Categories_Companies] CHECK CONSTRAINT [FK_Categories_Companies_CreatedBy]
GO
ALTER TABLE [dbo].[Categories_Companies]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Companies_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Categories_Companies] CHECK CONSTRAINT [FK_Categories_Companies_LastModifiedBy]
GO
ALTER TABLE [dbo].[Categories_Companies]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Companies2] FOREIGN KEY([companyID])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[Categories_Companies] CHECK CONSTRAINT [FK_Categories_Companies2]
GO
ALTER TABLE [dbo].[CommentRatings]  WITH CHECK ADD  CONSTRAINT [FK_CommentRatings_Comment] FOREIGN KEY([Comment])
REFERENCES [dbo].[Comments] ([ID])
GO
ALTER TABLE [dbo].[CommentRatings] CHECK CONSTRAINT [FK_CommentRatings_Comment]
GO
ALTER TABLE [dbo].[CommentRatings]  WITH CHECK ADD  CONSTRAINT [FK_CommentRatings_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CommentRatings] CHECK CONSTRAINT [FK_CommentRatings_LastModifiedBy]
GO
ALTER TABLE [dbo].[CommentRatings]  WITH CHECK ADD  CONSTRAINT [FK_CommentRatings_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CommentRatings] CHECK CONSTRAINT [FK_CommentRatings_User]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_ForCharacteristic] FOREIGN KEY([characteristicID])
REFERENCES [dbo].[ProductCharacteristics] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_ForCharacteristic]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_ForSubVariant] FOREIGN KEY([SubVariant])
REFERENCES [dbo].[ProductSubVariants] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_ForSubVariant]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_ForVariant] FOREIGN KEY([Variant])
REFERENCES [dbo].[ProductVariants] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_ForVariant]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_LastModifiedBy]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Users] FOREIGN KEY([userID])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Users]
GO
ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_CreatedBy]
GO
ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_LastModifiedBy]
GO
ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_Types] FOREIGN KEY([type])
REFERENCES [dbo].[CompanyTypes] ([ID])
GO
ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_Types]
GO
ALTER TABLE [dbo].[CompaniesCharacterestics]  WITH CHECK ADD  CONSTRAINT [FK_Companies_Characterestics] FOREIGN KEY([companyID])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[CompaniesCharacterestics] CHECK CONSTRAINT [FK_Companies_Characterestics]
GO
ALTER TABLE [dbo].[CompaniesCharacterestics]  WITH CHECK ADD  CONSTRAINT [FK_CompaniesCharacterestics_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CompaniesCharacterestics] CHECK CONSTRAINT [FK_CompaniesCharacterestics_CreatedBy]
GO
ALTER TABLE [dbo].[CompaniesCharacterestics]  WITH CHECK ADD  CONSTRAINT [FK_CompaniesCharacterestics_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CompaniesCharacterestics] CHECK CONSTRAINT [FK_CompaniesCharacterestics_LastModifiedBy]
GO
ALTER TABLE [dbo].[CompanyImages]  WITH CHECK ADD  CONSTRAINT [FK_CompanyImages_Company] FOREIGN KEY([compID])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[CompanyImages] CHECK CONSTRAINT [FK_CompanyImages_Company]
GO
ALTER TABLE [dbo].[CompanyImages]  WITH CHECK ADD  CONSTRAINT [FK_CompanyImages_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CompanyImages] CHECK CONSTRAINT [FK_CompanyImages_CreatedBy]
GO
ALTER TABLE [dbo].[CompanyTypes]  WITH CHECK ADD  CONSTRAINT [FK_CompanyTypes_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CompanyTypes] CHECK CONSTRAINT [FK_CompanyTypes_CreatedBy]
GO
ALTER TABLE [dbo].[CompanyTypes]  WITH CHECK ADD  CONSTRAINT [FK_CompanyTypes_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[CompanyTypes] CHECK CONSTRAINT [FK_CompanyTypes_LastModifiedBy]
GO
ALTER TABLE [dbo].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_Users] FOREIGN KEY([userID])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Logs] CHECK CONSTRAINT [FK_Logs_Users]
GO
ALTER TABLE [dbo].[NotifyOnNewContent]  WITH CHECK ADD  CONSTRAINT [FK_NotifyOnNewContent_ModifiedBy] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[NotifyOnNewContent] CHECK CONSTRAINT [FK_NotifyOnNewContent_ModifiedBy]
GO
ALTER TABLE [dbo].[NotifyOnNewContent]  WITH CHECK ADD  CONSTRAINT [FK_NotifyOnNewContent_User] FOREIGN KEY([notifyUser])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[NotifyOnNewContent] CHECK CONSTRAINT [FK_NotifyOnNewContent_User]
GO
ALTER TABLE [dbo].[ProductCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_ProductCharacteristics] FOREIGN KEY([productID])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductCharacteristics] CHECK CONSTRAINT [FK_ProductCharacteristics]
GO
ALTER TABLE [dbo].[ProductCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_ProductCharacteristics_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductCharacteristics] CHECK CONSTRAINT [FK_ProductCharacteristics_CreatedBy]
GO
ALTER TABLE [dbo].[ProductCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_ProductCharacteristics_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductCharacteristics] CHECK CONSTRAINT [FK_ProductCharacteristics_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductImages]  WITH CHECK ADD  CONSTRAINT [FK_ProductImages_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductImages] CHECK CONSTRAINT [FK_ProductImages_CreatedBy]
GO
ALTER TABLE [dbo].[ProductImages]  WITH CHECK ADD  CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([prodID])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductImages] CHECK CONSTRAINT [FK_ProductImages_Products]
GO
ALTER TABLE [dbo].[ProductLinks]  WITH CHECK ADD  CONSTRAINT [FK_ProductLinks_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductLinks] CHECK CONSTRAINT [FK_ProductLinks_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductLinks]  WITH CHECK ADD  CONSTRAINT [FK_ProductLinks_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductLinks] CHECK CONSTRAINT [FK_ProductLinks_Product]
GO
ALTER TABLE [dbo].[ProductLinks]  WITH CHECK ADD  CONSTRAINT [FK_ProductLinks_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductLinks] CHECK CONSTRAINT [FK_ProductLinks_User]
GO
ALTER TABLE [dbo].[ProductRatings]  WITH CHECK ADD  CONSTRAINT [FK_ProductRatings_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductRatings] CHECK CONSTRAINT [FK_ProductRatings_Product]
GO
ALTER TABLE [dbo].[ProductRatings]  WITH CHECK ADD  CONSTRAINT [FK_Ratings_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductRatings] CHECK CONSTRAINT [FK_Ratings_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductRatings]  WITH CHECK ADD  CONSTRAINT [FK_Ratings_User] FOREIGN KEY([user])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductRatings] CHECK CONSTRAINT [FK_Ratings_User]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([categoryID])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Companies] FOREIGN KEY([companyID])
REFERENCES [dbo].[Companies] ([ID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Companies]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_CreatedBy]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductInUnspecifiedCategory] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory] CHECK CONSTRAINT [FK_Product_ProductInUnspecifiedCategory]
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory]  WITH CHECK ADD  CONSTRAINT [FK_UnspecifiedCategory_ProductsInUnspecifiedCategory] FOREIGN KEY([Category])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory] CHECK CONSTRAINT [FK_UnspecifiedCategory_ProductsInUnspecifiedCategory]
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory]  WITH CHECK ADD  CONSTRAINT [FK_WantedCategory_ProductsInUnspecifiedCategory] FOREIGN KEY([WantedCategory])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[ProductsInUnspecifiedCategory] CHECK CONSTRAINT [FK_WantedCategory_ProductsInUnspecifiedCategory]
GO
ALTER TABLE [dbo].[ProductSubVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubVariants_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductSubVariants] CHECK CONSTRAINT [FK_ProductSubVariants_CreatedBy]
GO
ALTER TABLE [dbo].[ProductSubVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubVariants_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductSubVariants] CHECK CONSTRAINT [FK_ProductSubVariants_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductSubVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubVariants_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductSubVariants] CHECK CONSTRAINT [FK_ProductSubVariants_Product]
GO
ALTER TABLE [dbo].[ProductSubVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubVariants_Variant] FOREIGN KEY([Variant])
REFERENCES [dbo].[ProductVariants] ([ID])
GO
ALTER TABLE [dbo].[ProductSubVariants] CHECK CONSTRAINT [FK_ProductSubVariants_Variant]
GO
ALTER TABLE [dbo].[ProductTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProductTopics_LastCommentBy] FOREIGN KEY([lastCommentBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductTopics] CHECK CONSTRAINT [FK_ProductTopics_LastCommentBy]
GO
ALTER TABLE [dbo].[ProductTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProductTopics_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductTopics] CHECK CONSTRAINT [FK_ProductTopics_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProductTopics_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductTopics] CHECK CONSTRAINT [FK_ProductTopics_Product]
GO
ALTER TABLE [dbo].[ProductTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProductTopics_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductTopics] CHECK CONSTRAINT [FK_ProductTopics_User]
GO
ALTER TABLE [dbo].[ProductVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductVariants_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductVariants] CHECK CONSTRAINT [FK_ProductVariants_CreatedBy]
GO
ALTER TABLE [dbo].[ProductVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductVariants_LastModifiedBy] FOREIGN KEY([LastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ProductVariants] CHECK CONSTRAINT [FK_ProductVariants_LastModifiedBy]
GO
ALTER TABLE [dbo].[ProductVariants]  WITH CHECK ADD  CONSTRAINT [FK_ProductVariants_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Products] ([ID])
GO
ALTER TABLE [dbo].[ProductVariants] CHECK CONSTRAINT [FK_ProductVariants_Product]
GO
ALTER TABLE [dbo].[ReportComments]  WITH CHECK ADD  CONSTRAINT [FK_ReportComments_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ReportComments] CHECK CONSTRAINT [FK_ReportComments_LastModifiedBy]
GO
ALTER TABLE [dbo].[ReportComments]  WITH CHECK ADD  CONSTRAINT [FK_ReportComments_Report] FOREIGN KEY([report])
REFERENCES [dbo].[Reports] ([ID])
GO
ALTER TABLE [dbo].[ReportComments] CHECK CONSTRAINT [FK_ReportComments_Report]
GO
ALTER TABLE [dbo].[ReportComments]  WITH CHECK ADD  CONSTRAINT [FK_ReportComments_User] FOREIGN KEY([user])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[ReportComments] CHECK CONSTRAINT [FK_ReportComments_User]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_CreatedBy]
GO
ALTER TABLE [dbo].[SiteNews]  WITH CHECK ADD  CONSTRAINT [FK_SiteNews_CreatedBy] FOREIGN KEY([createdBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[SiteNews] CHECK CONSTRAINT [FK_SiteNews_CreatedBy]
GO
ALTER TABLE [dbo].[SiteNews]  WITH CHECK ADD  CONSTRAINT [FK_SiteNews_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[SiteNews] CHECK CONSTRAINT [FK_SiteNews_LastModifiedBy]
GO
ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_LastModifiedBy] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_LastModifiedBy]
GO
ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_User] FOREIGN KEY([user])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_User]
GO
ALTER TABLE [dbo].[TransferTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_TransferTypeActions_TypeActions] FOREIGN KEY([TypeAction])
REFERENCES [dbo].[UsersTypeActions] ([ID])
GO
ALTER TABLE [dbo].[TransferTypeActions] CHECK CONSTRAINT [FK_TransferTypeActions_TypeActions]
GO
ALTER TABLE [dbo].[TransferTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_TransferTypeActions_UserReceiving] FOREIGN KEY([UserReceiving])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TransferTypeActions] CHECK CONSTRAINT [FK_TransferTypeActions_UserReceiving]
GO
ALTER TABLE [dbo].[TransferTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_TransferTypeActions_UserTransfering] FOREIGN KEY([UserTransfering])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TransferTypeActions] CHECK CONSTRAINT [FK_TransferTypeActions_UserTransfering]
GO
ALTER TABLE [dbo].[TypeSuggestionComments]  WITH CHECK ADD  CONSTRAINT [FK_TypeSuggestionComments_Suggestion] FOREIGN KEY([Suggestion])
REFERENCES [dbo].[TypeSuggestions] ([ID])
GO
ALTER TABLE [dbo].[TypeSuggestionComments] CHECK CONSTRAINT [FK_TypeSuggestionComments_Suggestion]
GO
ALTER TABLE [dbo].[TypeSuggestionComments]  WITH CHECK ADD  CONSTRAINT [FK_TypeSuggestionComments_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeSuggestionComments] CHECK CONSTRAINT [FK_TypeSuggestionComments_User]
GO
ALTER TABLE [dbo].[TypeSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_TypeSuggestions_ByUser] FOREIGN KEY([ByUser])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeSuggestions] CHECK CONSTRAINT [FK_TypeSuggestions_ByUser]
GO
ALTER TABLE [dbo].[TypeSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_TypeSuggestions_StatusBy] FOREIGN KEY([StatusBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeSuggestions] CHECK CONSTRAINT [FK_TypeSuggestions_StatusBy]
GO
ALTER TABLE [dbo].[TypeSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_TypeSuggestions_ToUser] FOREIGN KEY([ToUser])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeSuggestions] CHECK CONSTRAINT [FK_TypeSuggestions_ToUser]
GO
ALTER TABLE [dbo].[TypeWarnings]  WITH CHECK ADD  CONSTRAINT [FK_TypeWarnings_ByAdmin] FOREIGN KEY([ByAdmin])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeWarnings] CHECK CONSTRAINT [FK_TypeWarnings_ByAdmin]
GO
ALTER TABLE [dbo].[TypeWarnings]  WITH CHECK ADD  CONSTRAINT [FK_TypeWarnings_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeWarnings] CHECK CONSTRAINT [FK_TypeWarnings_ModifiedBy]
GO
ALTER TABLE [dbo].[TypeWarnings]  WITH CHECK ADD  CONSTRAINT [FK_TypeWarnings_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[TypeWarnings] CHECK CONSTRAINT [FK_TypeWarnings_User]
GO
ALTER TABLE [dbo].[TypeWarnings]  WITH CHECK ADD  CONSTRAINT [FK_TypeWarnings_UserTypeAction] FOREIGN KEY([UserTypeAction])
REFERENCES [dbo].[UsersTypeActions] ([ID])
GO
ALTER TABLE [dbo].[TypeWarnings] CHECK CONSTRAINT [FK_TypeWarnings_UserTypeAction]
GO
ALTER TABLE [dbo].[UsersTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_UsersTypeActions_ApprovedBy] FOREIGN KEY([approvedBy])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[UsersTypeActions] CHECK CONSTRAINT [FK_UsersTypeActions_ApprovedBy]
GO
ALTER TABLE [dbo].[UsersTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_UsersTypeActions_TypeActions] FOREIGN KEY([actionID])
REFERENCES [dbo].[TypeActions] ([ID])
GO
ALTER TABLE [dbo].[UsersTypeActions] CHECK CONSTRAINT [FK_UsersTypeActions_TypeActions]
GO
ALTER TABLE [dbo].[UsersTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_UsersTypeActions_User] FOREIGN KEY([userID])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[UsersTypeActions] CHECK CONSTRAINT [FK_UsersTypeActions_User]
GO
ALTER TABLE [dbo].[Visits]  WITH CHECK ADD  CONSTRAINT [FK_Visits_User] FOREIGN KEY([User])
REFERENCES [dbo].[UserIDs] ([ID])
GO
ALTER TABLE [dbo].[Visits] CHECK CONSTRAINT [FK_Visits_User]
GO
USE [master]
GO
ALTER DATABASE [wiadvice_siteDB_EN] SET  READ_WRITE 
GO

USE [wiadvice_siteDB_EN]
GO

INSERT INTO [dbo].[UserIDs] ([ID],[haveNewContent])
       VALUES (1 , 0) -- system
	   , (2 , 0) -- guest
	   , (3 , 0) -- admin 
GO

INSERT INTO [dbo].[CompanyTypes] ([name],[visible],[createdBy],[dateCreated],[lastModifiedBy],[lastModified],[description])
     VALUES ('other',1,1,GETUTCDATE(),1,GETUTCDATE(),'If other types don`t suit choose this one. ')
	 , ('company',1,3,GETUTCDATE(),3,GETUTCDATE(),'For all companies.')
GO

INSERT [dbo].[Companies] ([type], [name], [visible], [dateCreated], [description], [createdBy], [lastModifiedBy], [lastModified], [website], [canUserTakeRoleIfNoEditors]) 
	VALUES (2, N'other', 1, GETUTCDATE(), N'All products who are from companies that are not added will be from this..', 1, 1, GETUTCDATE(), NULL, 1)
GO

SET IDENTITY_INSERT [dbo].[Categories] ON 
GO
INSERT [dbo].[Categories] ([ID], [parentID], [name], [dateCreated], [last], [description], [createdBy], [lastModifiedBy], [lastModified], [visible], [imageUrl], [imageWidth], [imageHeight], [displayOrder]) 
VALUES 
(1, 17, N'Vehicles', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
, (3, 1, N'Automobiles', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(5, 1, N'Motorcycles', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(6, 1, N'Aircrafts', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 8)
,(7, 1, N'Boats', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(8, 17, N'Electronics', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(9, 8, N'Computers', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(10, 330, N'GSM', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(11, 66, N'TV', GETUTCDATE(), 1, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(15, 3, N'Standard', GETUTCDATE(), 1, N'A hand-held mobile radiotelephone for use in an area divided into small sections, each with its own short-range transmitter/receiver.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(16, 3, N'Smartphones', GETUTCDATE(), 1, N'', 1, 1, GETUTCDATE(), 1,NULL, NULL, NULL, 0)
,(17, NULL, N'Categories', GETUTCDATE(), 0, N'root', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(18, 5, N'Motorcycles', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(19, 18, N'Standard', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(20, 9, N'Notebooks', GETUTCDATE(), 1, N'­Also called laptops, notebooks are portable computers that integrate the display, keyboard, a pointing device or trackball, processor, memory and hard drive all in a battery-operated package slightly larger than an average hardcover book.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(21, 6, N'Airplanes', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(22, 7, N'Motor boats', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(27, 18, N'Sport', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(28, 18, N'Off-road', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(32, 3, N'Sports', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 0)
,(33, 3, N'Cars', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(34, 3, N'Limousines', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(37, 1, N'Trucks', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(38, 1, N'Other vehicles', GETUTCDATE(), 1, N'Vehicles that do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(39, 1, N'Light Trucks', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(40, 39, N'Minivans', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(41, 39, N'SUVs', GETUTCDATE(), 1, N'Vehicles similar to a station wagons, but built on a light-truck chassis. SUVs are with weaker suspension than off-road vehicles, not all have 4x4 and they primarily move on pavement roads. Off-road capabilities aren`t their first objective when they are built. Typical SUVs are: Audi Q7; BMW x5/x6; Mercedes-Benz M-Class; Honda CR-V.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(42, 39, N'Pickup trucks', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(43, 39, N'Vans', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(44, 5, N'ATVs', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(45, 6, N'Helicopters', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 0)
,(46, 6, N'Balloons', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(47, 6, N'Others', GETUTCDATE(), 1, N'Aircrafts that do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(48, 7, N'Yachts', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(49, 7, N'Speed and sport', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(50, 9, N'Desktop', GETUTCDATE(), 1, N'­­A PC that is not designed for portability is a desktop computer. The expectation with desktop systems are that you will set the computer up in a permanent location. Most desktops offer more power, storage and versatility for less cost than their portable brethren.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(52, 9, N'Servers', GETUTCDATE(), 1, N'A computer that has been optimized to provide services to other computers over a network. Servers usually have powerful processors, lots of memory and large hard drives.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(53, 9, N'Other', GETUTCDATE(), 1, N'Here you can add and find computers, which do not belong to the predefined computer categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(55, 65, N'MP3 players', GETUTCDATE(), 1, N'We all have favorite music and singers – Mp3 players are one of the devices with which we can listen to them.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(56, 8, N'Cameras', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(57, 56, N'Photo cameras', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(63, 56, N'Surveillance cameras', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(64, 56, N'Camcorders', GETUTCDATE(), 1, N'With them we record favorite and memorable moments, shots from event`s place and other. Professional or semi professional - their place is here.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(65, 8, N'Audio', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(66, 8, N'Video', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(67, 8, N'Other electronics', GETUTCDATE(), 1, N'Electronic products, which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(69, 17, N'Music', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(72, 9, N'Hardware', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(78, 79, N'Scanners', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(79, 72, N'Periphery', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(80, 72, N'Inside', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(81, 79, N'Printers', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(82, 79, N'Monitors', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(84, 79, N'Gaming periphery', GETUTCDATE(), 1, N'Extras that bring gaming experience closer to reality :) - steering wheels, joysticks, joypads and others.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(85, 79, N'Other', GETUTCDATE(), 1, N'Here you can add and find peripheral hardware, which does not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(86, 79, N'Keyboards', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(87, 79, N'Mice', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(88, 80, N'Video controllers', GETUTCDATE(), 1, N'Powerful video card is the main thing, that`s needed for the proper work of the latest games and graphic processing programs.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(89, 80, N'Hard drives', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(90, 80, N'Memory chips', GETUTCDATE(), 1, N'If you need better performance for your computer, this is probably one of the things you`ll need to upgrade.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(91, 80, N'Disc roms', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(92, 80, N'CPU', GETUTCDATE(), 1, N'Computer`s central processing unit determines its processing capabilities. Faster processor = better performance.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(93, 80, N'Motherboards', GETUTCDATE(), 1, N'A motherboard is the central printed circuit board and holds many of the crucial components of the system, while providing connectors for other peripherals. Motherboards determine what can be the rest of the computer`s configuration, so choose them carefully.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(95, 80, N'Other', GETUTCDATE(), 1, N'Here you can add and find inside hardware, which does not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(99, 79, N'Network periphery', GETUTCDATE(), 1, N'Routers, switches, modems and other devices that connect us to a network - look for them here.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(100, 359, N'Binoculars', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(101, 17, N'Engineering', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(102, 101, N'Household', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(103, 102, N'Washing machines', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(104, 102, N'Refrigerators', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(105, 102, N'Cookers', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(106, 102, N'Water-heaters', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(111, 102, N'Coffee machines', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(129, 101, N'Air conditioners', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(141, 17, N'Furniture', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(144, 141, N'Chairs', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 8)
,(160, 144, N'Ordinary chairs', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(161, 144, N'Office chairs', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(163, 144, N'Other', GETUTCDATE(), 1, N'Chairs which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(186, 330, N'GPS', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(322, 3, N'Others', GETUTCDATE(), 1, N'Cars which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 3)
,(324, 8, N'Game consoles', GETUTCDATE(), 1, N'From 5 to 55 years we all like to play electronic games – game consoles give us the possibility to enjoy in front of TV in our free time.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 8)
,(325, 65, N'Speakers', GETUTCDATE(), 1, N'High quality sound and music – every music fan`s dream – speakers in our home or in favorite disco carry us to the dreamlands.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(326, 66, N'Projectors', GETUTCDATE(), 1, N'They are the modern choice for school or business purposes, presentation or advertisements, because of their mobility and large image scale.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(330, 8, N'Communication', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(331, 330, N'Phones', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(332, 330, N'Fax machines', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(333, 66, N'Home theater', GETUTCDATE(), 1, N'Live concert, film premiere in home, or football game – you receive full sensation for what`s happening with home theater systems.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(334, 65, N'Mini systems', GETUTCDATE(), 1, N'They bring together in one – radio, cd-changer, speakers, remote control, other extras, and deliver pleasure with their functionality.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(335, 65, N'Accessories', GETUTCDATE(), 1, N'Headphones, microphones and other products needed for, or adding to our pleasure from music or other audio information.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(336, 66, N'Players', GETUTCDATE(), 1, N'They give you the possibility to watch films, concerts or other video information written on DVD, Blu-ray or other carriers on the TV.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(338, 8, N'Car electronics', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(339, 9, N'Netbooks', GETUTCDATE(), 1, N'Small notebook with fewer possibilities.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(340, 9, N'Tablet PCs', GETUTCDATE(), 1, N'Small portable computers with sensor display, which is their main input device. Built mainly for internet browsing and book reading.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(341, 17, N'Construction', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(342, 341, N'Building machines', GETUTCDATE(), 1, N'Diggers, concrete trucks, road rollers and any other type and size building machinery.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(343, 341, N'Building materials', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(344, 341, N'Electrical tools', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(345, 341, N'Hand tools', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(346, 341, N'Others', GETUTCDATE(), 1, N'Here you can add and find construction machines and tools, which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(347, 341, N'Precast units', GETUTCDATE(), 1, N'Precast units and prefabricated houses.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(348, 343, N'Dry construction mixtures', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 1)
,(349, 343, N'Others', GETUTCDATE(), 1, N'Here you can add and find building materials, which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(350, 343, N'Paints and varnishes', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(351, 343, N'Primers and plasters', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(352, 343, N'Bathroom and sanitary', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(353, 101, N'Agricultural', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(354, 101, N'Garden', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(355, 353, N'Combines', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(356, 353, N'Others', GETUTCDATE(), 1, N'Agricultural machines, which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(357, 353, N'Tractors', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(358, 102, N'Others', GETUTCDATE(), 1, N'Here you can add and find household engineering, which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(359, 17, N'Others', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(360, 359, N'Glasses', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(361, 359, N'Watches', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(362, 141, N'Bathroom', GETUTCDATE(), 1, N'Here you can find/add furniture for bathrooms. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(363, 141, N'Bedroom', GETUTCDATE(), 1, N'Here you can find/add furniture for bedrooms. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(364, 141, N'Children', GETUTCDATE(), 1, N'Here you can find/add furniture for children rooms. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(365, 141, N'Kitchen', GETUTCDATE(), 1, N'Here you can find/add furniture for kitchens. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(366, 141, N'Living-room', GETUTCDATE(), 1, N'Here you can find/add furniture for living-rooms. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(367, 141, N'Office', GETUTCDATE(), 1, N'Here you can find/add furniture for offices. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(368, 141, N'Other', GETUTCDATE(), 1, N'Furniture which do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(369, 17, N'Sport', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(370, 69, N'Accessories and others', GETUTCDATE(), 1, N'Professional microphones, strings, light equipment and other accessories/products, which are needed for concerts, theatrical and film activities.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(371, 69, N'Instruments', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(373, 69, N'Sound engineering', GETUTCDATE(), 1, N'Products used in concerts, theatrical and film activity, and other mass activities – mixers, turntables, DJ equipment, and other professional and semi-professional sound engineering.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(374, 371, N'String instruments', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(375, 371, N'Other', GETUTCDATE(), 1, N'Music instruments which don''t belong to other categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(376, 371, N'Electric instruments', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(377, 371, N'Keyboard instruments', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(378, 371, N'Percussion instruments', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(379, 371, N'Wind instruments', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(380, 369, N'Basic accessories', GETUTCDATE(), 1, N'Objects, without which the different sports can`t be practiced – balls for football, basketball and other, tennis racquets, sticks, skis… - You got the idea. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(381, 369, N'Fitness apparatus', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(382, 369, N'Hunt and fishing', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(383, 369, N'Accessories and others', GETUTCDATE(), 1, N'Other products, which make sport practicing safer and enjoyable.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(384, 382, N'Accessories', GETUTCDATE(), 1, N'Here you can add and find every other equipment for fishermen and hunters. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(385, 382, N'Fishing rods', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(386, 382, N'Hunt weapons', GETUTCDATE(), 1, N'Mainly smoothbore guns, hunting knifes, crossbows and others.

It`s strictly forbidden to add war weapons!
', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(387, 7, N'Personal watercrafts', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(388, 7, N'Others', GETUTCDATE(), 1, N'Boats that do not belong to the predefined categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(389, 1, N'Buses', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(390, 1, N'Bicycles', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(391, 1, N'Tyres and accessories', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(392, 39, N'Mini-buses', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(393, 39, N'Off-road', GETUTCDATE(), 1, N'Designed primarily for move on off-roads and normal roads. Off-road vehicles have better suspension and gears than SUVs, 4x4, sturdy tires and others. Typical off-road light trucks are: Lada Niva; Hummer H1; Land Rover Defender; Jeep Wrangler.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(394, 5, N'Mopeds', GETUTCDATE(), 1, N'Mopeds are typically restricted to 50 km/h (30 mph) and a maximum engine displacement of 49 cc (3.0 cu in).', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 2)
,(395, 5, N'Scooters', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1,NULL, NULL, NULL, 3)
,(397, 65, N'Other', GETUTCDATE(), 1, N'Here you can add and find audio electronics products, which do not belong to the predefined audio electronics categories.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(398, 141, N'Floor covering', GETUTCDATE(), 1, N'For all types of floor covering.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(399, 341, N'Window and door frames', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(400, 48, N'Sailing yachts', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(401, 48, N'Motor yachts', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(402, 369, N'Sport shoes', GETUTCDATE(), 1, N'Shoes used by different sports.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(403, 8, N'Security and signal', GETUTCDATE(), 1, N'Electronics connected with safety and security of infrastructure, private and country property – scanners, detectors, alarms, light signalization and others.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(404, 359, N'Unspecified', GETUTCDATE(), 1, N'Here are the products, for which editors of companies haven`t added the respective categories.
You can only add product here for an existing “company” (pre-selected).
For more information, see the guide.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO

INSERT [dbo].[SiteNews] ([name], [description], [dateCreated], [type], [visible], [createdBy], [lastModified], [lastModifiedBy], [linkID]) 
VALUES 
(N'about', N'&nbsp;&nbsp; Hello! We created this site, in order users to be able to express opinions about the products they buy. We are sure that you make difference between discussion and opinions – we won`t discuss, we will measure the quality of the products! A distinction is that you will be able to add companies and products in which you are interested!<br />
<br />
<div style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: right; ">If you want to know more about the idea <a href="AboutUs.aspx" target="_blank">click here</a>.<br />
Information for work with the site and its structure is <a href="Guide.aspx" target="_blank">placed here</a>.</div>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'3')
,(N'aboutExtended', N'<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp; Every day we are flooded with various types of advertisements, which “assure” us that we can`t live without this or that product. Besides that, companies try to hide facts, which lower the prestige of them and their products. In addition, a big part of the products isn’t anymore that, for which they are advertised.</div> <br />
<div style="background-color:#edf5fe; padding:5px;">&nbsp;&nbsp; <span style="font-style: italic; ">There are many places in which problems with products are discussed, but actual information about their quality takes some time to be found, mainly because it’s scattered. That’s why, a place is needed, in which such information can be collected and found.</span></div> <br />
<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp;&nbsp;In order to save time (and nerves!) the information, which users will share in this site should only be in form of&nbsp;opinions and comments – without redundant verboseness and product comparisons.<br />
</div> <br />
<div style="background-color:#edf5fe; padding:5px;">&nbsp;&nbsp; <span style="font-style: italic; ">Users can add products and companies always according to their wishes. But, as you know there can’t be “possibility without responsibility”, and because of that we assure you, the site will be moderated always and the administrators will watch that the rules are followed. And here we will always look for your help, to control the incorrect users – please, when you find irregularity and/or rule breaking, report to the administrators immediately.</span></div> <br />
<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp;<span style="font-weight: bold; "> We tried our best to build innovatory product, which is interesting, because you are developing, controlling and building it. Will you take the possibility?</span></div>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'4')
,(N'suggestionsAbout', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #003399; font-size: 16pt; font-family: times new roman, times, serif; ">Your opinions about how the site should change to be better</span></p> <br />
 &nbsp;&nbsp;Suggestions posted from this page can be seen from everyone. Only users registered with email can post.<br />
<span style="color: #cc3300; ">&nbsp;&nbsp;The opinions will be moderated if they are not about the site, or contain inappropriate language.</span>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'5')
,(N'aboutFAQ', N'&nbsp;&nbsp; Here will be listed most frequently asked questions about the site along with their answers. That’s why if you have questions this is the page where you should look for answers as well as <a shape="rect" href="Guide.aspx" target="_blank">guide</a>, <a shape="rect" href="Rules.aspx" target="_blank">rules</a>, and if you don’t find any, don’t hesitate to ask us from the form in <a shape="rect" href="AboutUs.aspx" target="_blank">about page</a>.&nbsp;<br />
<br />
<p style="text-align: center; margin: 0px">&nbsp;The Questions and Answers will be populated in time.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'6')
,(N'aboutRules', N'&nbsp;&nbsp; By default users visit web sites to gather/share information or have fun. This is valid for this web site, and for that it is necessary couple of rules to be followed, in order time spent with the site to be pleasant. <br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "> Please read and follow the rules when interacting with the site and report those users who aren`t.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'7')
,(N'Rules and notes for posting opinions', N'<ul style="padding-left:30px;">
<li>Opinions not related to the chosen product are not allowed;</li>
<li>Offensive language is strictly forbidden;</li>
<li>The only opinions language should be English;</li>
<li>User attacks in opinions are not allowed;</li>
<li>If a user wants to express opinion which is already written by other user, it`s advised to just rate that opinion instead of writing a new one.</li></ul>&nbsp;&nbsp;&nbsp;<span style="color: #009900">Notes : </span><br />
1. The number of opinions that a user can post for a product is limited.<br />
 2. Newly registered users and guests will have to wait limited time before posting next opinion. This is done to slow down possible spam attacks. After registered users reach certain number of opinions, that time restriction will removed. <br />
 3. Users should mark as violation every opinion, which isn`t following the rules, in order to keep the opinions clear from unnecessary or unrelated information.<br />
 <br />
&nbsp;&nbsp; Failure to follow the rules will lead to sanctions.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29;"><u>Site team is not responsible for the information, posted and shared by the users!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulescomms')
,(N'registration', N'<p style="text-align: center; margin: 0px"><span style="font-family: times new roman,times,serif; color: #003399; font-size: 16pt">With registration you will gain the following possibilities :</span></p><br />
1. Participation in each product''s forum (posting topics and comments).<br />
2. Send private messages to other users.<br />
3. Add products and companies to the site.<br />
4. Rate products and opinions.<br />
5. Write suggestions for the site.<br />
6. Report irregularities with site or report users who don`t follow the rules.<br />
7. Send suggestions about products/companies to their editors.<br />
<br />
&nbsp;&nbsp; <span style="color: #c02e29; ">There are two types of registrations, with or without specifying your email.</span> If you choose to register without email you won`t have rights 3, 5, 7.<br />
<br />
&nbsp;&nbsp; More information for registration can be <a shape="rect" href="Guide.aspx#infar" target="_blank">read here</a>.<br />
&nbsp;&nbsp; With registering you are agreeing with the <a shape="rect" href="Rules.aspx" target="_blank">rules</a> to use this site.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'13')
,(N'aboutAdmins', N'&nbsp;&nbsp; When you register administrators, you will only be able to grant them levels lower than the level you have, except for global administrators. Select only the needed roles for new administrator, and do not register if there is no need to.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'19')
,(N'aboutAdverts', N'&nbsp;&nbsp; Advertisements are managed from here. Use it to add/edit/delete advertisements. If you are not familiar how to work with advertisements read the Fields information.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009900">Field information</span></p><br />
1. Html : Type HTML code for the advertisement if it is required. Do not forget to include the code for showing the advertisement. If this field text (is populated?) then the information in ‘Upload File’ and ‘Advert url’ will not have any effect. If HTML code is not required, DO NOT populate this field.<br />
 2. Upload File : If the Advertisement is a multimedia file (image/flash) then select from where it will be uploaded, because the file needs to be on the server.<br />
 3. Advert URL : If file is going to be uploaded from ‘Upload File’ field then you need to type to which web site (URL) the user will be redirected when he clicks on the advertisement.<br />
 4. Category IDs : Type the IDs for which categories (and their subcategories) the advertisement will be shown. This field can be empty.<br />
 5. Company IDs : Type the IDs of companies in which the advertisement should show, it will be also shown in products of these companies. This field can be empty.<br />
 6. Product IDs : Type the IDs of products in which the advertisement should show. This field can be empty.<br />
 <br />
 <span style="font-style: italic; ">Note : When you edit advertisement, if you want the old links (for categories, companies, products) to be preserved, type them again or they will be deleted.</span><br />
 <br />
 7. Expire date : Type the date until which the advertisement will be active, if there is no such date leave the field empty. (Specify here whether expire date is included in the period when the advertisement is active.)<br />
 8. Active : Check it if the advertisement should show in the site`s pages when it`s added. If it isn`t checked the advertisement will be added to database but it will not show in site`s pages.<br />
 9. General : Check this if the advertisement should show in all pages. If none of the Company/Product/Category ID fields are filled then you need to check General.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'20')
,(N'aboutLogs', N'&nbsp;&nbsp; Logs are shown here. Choose criteria from the options and click Show button to show the Logs.
<p style="text-align: center; margin: 0px"><br />
</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'21')
,(N'aboutAddCompany', N'<ul style="padding-left:30px;">
<li>When you add company you will&nbsp;automatically become it`s editor. That means you will be able to modify it;</li>
<li><span style="color: #c02e29; ">After you add a company, you will have to add categories in which the company will be able to have products.</span> That is important because if you don’t add categories you/users won`t be able to add products to it. You will have option to add categories from the company`s page;</li>
<li>After you add company, you will be able to add images/characteristics/...&nbsp;You will be able to change company''s name up to 30&nbsp;minutes after it''s added;<br />
</li>
<li>You won`t be able to edit products which are added to this company, if you are not the one who added them.</li></ul>&nbsp;&nbsp; More information about adding/editing companies can be seen <a shape="rect" href="Guide.aspx#infaaemaker" target="_blank">here</a>.<br />
&nbsp;&nbsp; Rules about adding and editing company can be seen <a shape="rect" href="Rules.aspx#rulesaemaker" target="_blank">here</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'22')
,(N'aboutAddProduct', N'&nbsp;&nbsp; <span style="color: #009900">Couple of notes that you should be aware of :</span>
<ul style="padding-left:30px;">
<li>When you add the product you will automatically become it`s editor. That means you will be able to modify it.</li>
<li>After you add a product, you will be able to modify characteristics/images/variants/description/... from it`s page.&nbsp;You will be able to change product''s name/category/company up to 30 minutes after it''s added.</li></ul>&nbsp;&nbsp; More information about adding/editing products can be seen <a shape="rect" href="Guide.aspx#infaeproducts" target="_blank">here</a>.<br />
&nbsp;&nbsp; Rules about adding/editing products can be seen <a shape="rect" href="Rules.aspx#rulesaeprod" target="_blank">here</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'23')
,(N'aboutReports', N'<p style="text-align: center; margin: 0px">This page is for viewing/answering/resolving user reports about the site.</p><br />
<span style="color: #009900">When viewing report use the following buttons for :</span><br />
Make Viewed : Click it to indicate that the report has been seen by administrator and he is working on it.<br />
 Resolve : Use it to indicate that the report has been resolved, that way no more replies can be exchanged between the user and the administrators.<br />
 Reply : Use it to write reply to user about the report.<br />
 Delete comment : If it`s a spam report clicking this button will delete the comment and indicate it as resolved, it`ll also send warning to the user who wrote the comment.<br />
 Delete suggestion : Analogous to delete comment.<br />
 <br />
&nbsp;&nbsp; If the report is about edit suggestion, clicking on ‘Edit suggestion’ link will show the suggestion.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'24')
,(N'aboutSiteTexts', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #009900; ">This is the place where&nbsp;site texts can be modified.</span></p><br />
<span style="color: #009900">Different types when adding text mean :</span><br />
1. Text : Use it to add texts with names that are shown in the table for missing texts. If there are no missing texts then don’t use this type, because the text wont show in the site`s pages.<br />
 2. News : Use it to add news. News is shown in home page.<br />
 3. Question and answer: Adds new question and answer to FAQ page.<br />
 4. Rule : Adds new rule to rules page.<br />
 5. Guide text : Information on how to work with site. They are shown in Guide page.<br />
 6. Warning pattern : Text pattern used when sending warnings to users. They are shown in Warnings page.<br />
 7. Report pattern : Text pattern used when answering user reports. They are shown in reports page.<br />
 <br />
 Link ID : The id with which the text will be linked from other pages is the site. Currently used for rules, FAQ and guide texts.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'25')
,(N'aboutStatistics', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; ">Statistics and users currently online can be seen here.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'26')
,(N'aboutUserReporting', N'Please report irregularities only for current page.<br />
<br />
   You can write about bugs within page or inappropriate content. <br />
 Describe the problem as accurate as possible.<br />
<br />
   More information can be read <a shape="rect" href="Guide.aspx#infabreports" target="_blank">here</a>.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'27')
,(N'aboutGeneralReporting', N'From here you can write about bugs within site or inappropriate content. <br />
<br />
  Please type in which page you encountered the issue.<br />
  Describe the issue as accurate as possible in order to find it fast and react appropriate.<br />
<br />
  More information can be read&nbsp;<a shape="rect" href="Guide.aspx#infabreports" target="_blank">here</a>.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'28')
,(N'Information for adding/editing company', N'&nbsp;&nbsp;&nbsp;Adding companies allows an easier organization of the products in the site.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Add company information</span></p><br />
&nbsp;&nbsp; Only users which are registered with email can add companies.<br />
&nbsp;&nbsp; Companies can be added by visiting the link “Add company” in the menu.<br />
 <br />
&nbsp;&nbsp; When user adds company he automatically becomes it`s editor, which means he can modify it. The user`s Edit rights page contains a list of products/companies which can be modified by him.<br />
 <br />
 <span style="font-style: italic; ">Note : When user adds company he will be able to edit it, but not the products added to this company (if they are added by other users).</span><br />
<br />
  &nbsp; <span style="font-size: 13pt; color: #009933; ">Fields :</span><br />
<br />
1. Company name (Official company name)<br />
 2. Web site : Company`s official web site, to which the users will be lead when they want to learn more about the company. <br />
 3. Description : Brief description about the company, which will tell in short what is the company activity. If the description is long, most of it can be added later as characteristic.<br />
<br />
&nbsp;&nbsp; Characteristics, categories, images and alternative names can be added after the company is added.<br />
 <br />
 <span style="font-style: italic; ">Note :&nbsp;Company name can only be modified up to 30 minutes after the company is added. After that, only administrators will be able to modify it.</span><br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009933; font-size: 13pt; ">Edit company information</span></p><br />
&nbsp;&nbsp; Companies can be modified from their pages. If user has right to edit it (in most cases this means that he added it) there will be shown 2 options (ADD and EDIT) just after the company description.<br />
 <br />
1. Characteristics : In these fields any additional information related with the company can be filled.<br />
 2. Categories : In order products to be added to company, it is needed to be chosen in which categories the company can have products. This is necessary in order the company to be shown in the dropdown list when adding product.<br />
 <br />
 <span style="font-style: italic; ">Note : If categories in which company can have products are not added, users won`t be able to add products to it.</span><br />
 <br />
3. Images : A limited number of images can be added to every company. One of these images can be marked as logo. The logo will always be displayed in front on the company`s page and other places.<br />
4.&nbsp;<span style="color: #333333; ">Alternative name :&nbsp;Alternative name&nbsp;with which the company is known.&nbsp;Example : product &quot;Sony Playstation 3&quot; is also known as &quot;PS 3&quot;; company &quot;LG&quot; is known as &quot;Lucky-Goldstar&quot; and &quot;Lucky&quot;.&nbsp;Companies can be found also by their alternative names.<br />
<br />
</span>
<p style="text-align: left; margin: 0px">&nbsp;&nbsp; Rules for adding/editing company can be <a shape="rect" href="Rules.aspx#rulesaemaker" target="_blank">read here</a>.</p>', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infaaemaker')
,(N'Information for registration', N'&nbsp;&nbsp;Users which want to have more possibilities will have to register.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Possibilities which users receive after they register :</span></p><br />
1.&nbsp;Participation in each product''s forum (posting topics and comments).<br />
2. Send private messages to other users. <br />
 3. Add companies and products. <br />
 4. Rate products and opinions.<br />
5. Write suggestions for the site in Suggestions section.<br />
6. Report irregularities or users who break the rules.<br />
7. Send suggestions about products/companies to their editors. <br />
 <br />
&nbsp;&nbsp; There are two types of registration which difference is in the rights they get.<br />
&nbsp;Writer : He will get rights 3, 5, 7, but email won`t be needed.<br />
 &nbsp;User : User with all rights. Valid email will need to be specified.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009933; font-size: 13pt; ">Registration fields</span></p><br />
1. Username : The name with which user will be associated in the site.<br />
 <br />
 <span style="font-style: italic; text-decoration: underline; ">Important : It`s forbidden to use offensive words for username.</span><br />
 <br />
2. Password : Account password. The stronger the better. <br />
3. Repeat password : Same password as above. This is precaution that the wanted password is typed.<br />
4. Email (User`s email address) - If email is typed user will be with full rights, otherwise not, as written above.<br />
 <br />
 <span style="font-style: italic; ">Note : If user don`t type his actual email address he won`t be able to activate his account, because link for activation will be send to the specified email. It will be used also if password is forgotten.&nbsp; </span><br />
 <br />
5. Secret question : Question which the user will have to answer if he forgets his password, in order to receive new password. <br />
6. Question`s answer : The answer to the secret question.<br />
7. CAPTCHA : In this field the letters which are shown in the image should be typed. Letter case doesn`t matter. This is a site defense against malicious programs (you must be a human in order to solve the CAPTCHA).<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">After registration</span></p><br />
&nbsp;&nbsp; After successful registration the system will log the user automatically. New options will appear in the site header and menu. <br />
&nbsp;&nbsp; Personal data like password, email, etc... can be modified from user`s profile page.<br />
 <br />
&nbsp;&nbsp;<span style="font-style: italic; "> It is important to know that all rights which user will get with registration can be removed by administrators, as well as deleting of his account, if the user breaks the site rules.</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infar')
,(N'Information for private messages', N'&nbsp;&nbsp; Because the site`s base are users and their opinions, it is possible users to exchange information privately in form of private messages.<br />
 <br />
&nbsp;&nbsp; User needs to be registered and logged in to send private messages.<br />
 <br />
&nbsp;&nbsp; Messages can be sent from user`s private messages page and from product pages by choosing an action from the “action” dropdown list of the respective opinion.<br />
 <br />
 <span style="font-style: italic; ">Note : Messages cannot be sent to: moderators; guests; users who have author blocked; users who have their message box blocked.</span><br />
 <br />
&nbsp;&nbsp; Received and sent messages can be seen in user`s private messages page. When new message is received an image will appear (letter) near profile and log out links.&nbsp;<br />
<br />
&nbsp;&nbsp; Block button : Will block the typed user, that way he won`t be able to sent you more messages.<br />
 <br />
&nbsp;&nbsp; Block/Unblock message box:<br />
 &nbsp; 1. Block message box : Blocks the message box for messages from other users, meaning they won`t be able to send any to the receiver.<br />
 <br />
 <span style="font-style: italic; ">Note : That doesn`t count for support, their messages are received always.</span><br />
 <br />
 &nbsp; 2. Unblock message box : Opposite of block message box. Have in mind that the blocked users will remain blocked.<br />
 <br />
 <span style="font-style: italic; ">Note : Moderators can’t be blocked.</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infapm')
,(N'aboutCompanyTypes', N'<p style="text-align: center; margin: 0px">Maker types are modified from here.</p>
<p style="text-align: left; margin: 0px"><br />
</p>Description must be short as possible, 3-4 rows at maximum.<br />
<br />
<span style="font-style: italic">NOTE : If you delete maker type with which there are makers then they`ll become of type ''other''.</span>', GETUTCDATE(), N'text', 1, 3,GETUTCDATE(), 3, N'33')
,(N'aboutLogIn', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #003399; ">Log in to be able to :</span> </p><br />
  &nbsp;&nbsp; • Rate products and opinions, send messages;<br />
  &nbsp;&nbsp; • Add products and companies<span style="color: #ff0000">*</span>, write opinions;<br />
  &nbsp;&nbsp; • Write suggestions, report inappropriate;<br />
&nbsp;&nbsp; • Participate in each product''s forum;<br />
<p style="margin: 0px 0px 0px 240px">… and more.</p><br />
&nbsp;&nbsp; If you forgot your password, <a shape="rect" href="ForgottenPassword.aspx" target="_blank">click here</a>. <br />
&nbsp;&nbsp; If you want to register visit <a shape="rect" href="Registration.aspx" target="_blank">this page</a>.<br />
<br />
<span style="color: #ff0000; ">&nbsp;&nbsp; *</span>&nbsp;Only if you are registered with email.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'34')
,(N'Information for adding/editing product', N'&nbsp;&nbsp; Users need to add the products for which they want to post opinions.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Add product information</span></p><br />
&nbsp;&nbsp; Products can be added by users which are registered with email address.<br />
<br />
&nbsp;&nbsp; When user adds a product, he automatically becomes his editor, which means he can modify it. The user`s Edit rights page contains a list of products/companies which can be modified by him.<br />
<br />
&nbsp;&nbsp;&nbsp;<span style="color: black">There are four places from which product can be added :</span><br />
1. Click on “Add product” option in the menu.<br />
2. Go to the category in which you want to add product and click on “Add product” link.<br />
3. Go to the company page for which you want to add product and click on “Add product” link. <span style="color: black">Hint:</span><span style="color: #0070c0"> </span><span style="color: black">To find the necessary company page, type the company name in the field next to the site logo and press the “Search” button. Then click on the respective link in the list of companies that match your search criterion.<br />
4. Go to the product page and click on “Add product” link. Hint: First, find an existing product of the same company as the company of the product you want to add.</span><br />
<br />
&nbsp;&nbsp; <span style="font-size: 13pt; color: #009933; ">Fields :</span><br />
<br />
1. Name (The name of the product) - If the name consist mostly of numbers and/or single characters, the name of the company can be added in front of the product name (example : “Fictive 5623k”, where “Fictive” is he company name and “5623k” is the product name).<br />
2. Company (Product`s company) - If product is being added to company, this field will be disabled and the company will be selected. <br />
<br />
<span style="font-style: italic">Note : If you don`t see the wanted company in the list, it means that it can`t have products in this category. Company editors can choose in which categories it can have products.&nbsp;For more information, </span><a shape="rect" href="Guide.aspx#infts" target="_blank"><span style="font-style: italic">click here</span></a><span style="font-style: italic">.</span><br />
<br />
3. Category (Category in which the product will be) - If the product is being added to a category this field will be disabled and the category will be selected. If product is being added to a company, there will be a list of category choices in which the company can have products.<br />
<br />
<span style="font-style: italic; color: black">Note: If the wanted product`s “category” is missing in the dropdown list, you can :</span>
<ul style="padding-left:30px;">
<li><span style="font-style: italic; ">send suggestion to company`s editor (to add the respective category);</span></li>
<li><span style="font-style: italic; ">choose the “Others -&gt; Unspecified” category from the dropdown menu and select appropriate product`s category from the “Categories” menu which will appear below. In this case the company`s editor will receive a system message for your request (the product to be in the wanted “category”) and if he adds the wanted category, the product will be moved to it.</span></li></ul><span style="font-style: italic">Warning : Please use the option correctly, in order to not get sanctioned! </span><br />
<br />
4. Web site -&nbsp;<span style="color: #333333; ">The official product web site. If you don''t know it leave the field empty.</span><br />
5. Description (Brief description of the product) If the product have technical data which isn`t long it can be pasted also, otherwise put it as characteristic. The maximum length of this field is limited, in order to be shown only most important information. The rest can be added as a characteristic.<br />
<br />
&nbsp;&nbsp; Images, characteristics, variants and alternative names can be added after product is added.<br />
<br />
<i>Note :&nbsp;</i><div style="display: inline; border-top-style: none; border-right-style: none; border-bottom-style: none; border-left-style: none; "><i>Name, category and&nbsp;</i><i>company can only be changed up to 30 minutes after the product is added. After that, only administrators will be able to modify them.</i></div><br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Edit product information</span></p><br />
&nbsp;&nbsp; Products can be modified from their pages. If the user has right to edit it (in most cases that mean that he added it) there will be shown 2 options (ADD and EDIT) just after the product description.<br />
<br />
1. Characteristics : Any additional information about the product can be put as a characteristic. For example, specific technical data, problems with the product and other. Users can give opinions for characteristics also. Opinions can be sorted by characteristic. Characteristics can be modified after they are added.<br />
<br />
<span style="font-style: italic">Note : If there are opinions for characteristic, then editors won`t be able to change that characteristic name, but they still will have the option to delete it. </span><br />
<br />
2. Images : A limited number of images can be added to every product. One of these images can be marked as main. Main image will always show in front on the product`s page and other places. Images can be deleted.<br />
3. Variants : In order to prevent product duplicating (adding) with same name and company, but different data, users have the option to add variants to products.&nbsp; Variants have description, in which the differences with main product can be described.<br />
<br />
<span style="font-style: italic">Example : Main product notebook Fictive A9M with variants : KL – for 4gb ram; KS – for 8gb ram.</span><br />
<br />
4. Sub-variants : Variants can have sub-variants with more specific differences.&nbsp; <br />
<br />
<span style="font-style: italic">Example : Variant Fictive A9M-KL with sub-variants : 23 – for 500gb hard disk; 24 – for 1tb hard disk.</span><br />
<br />
&nbsp;&nbsp; Users can give opinions for variants and sub-variants, opinions can be sorted by variants and sub-variants. Their descriptions can be modified after adding.<br />
<br />
<span style="color: #333333; ">5.&nbsp;Alternative name :&nbsp;Alternative name&nbsp;with which the product is known.&nbsp;Example : product &quot;Sony Playstation 3&quot; is also known as &quot;PS 3&quot;; company &quot;LG&quot; is known as &quot;Lucky-Goldstar&quot; and &quot;Lucky&quot;.&nbsp;Products can be found also by their alternative names.<br />
</span><br />
&nbsp;&nbsp; Rules for adding/editing product can be <a shape="rect" href="Rules.aspx#rulesaeprod" target="_blank">read here</a>.', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infaeproducts')
,(N'Information for type suggestions', N'&nbsp;&nbsp; Suggestions about products/companies allow users to contact editor and suggest him to update the information regarding product/company. Since some of the products are being updated in time (new variants are produced), information in the site needs to be updated, and this is the means for the users to remind editors that this is needed.<br />
 <br />
&nbsp;&nbsp; These suggestions can be sent only by users which are registered with email.<br />
 <br />
&nbsp;&nbsp; Suggestion about product can be sent from product`s page. The option is located above the write comment panel.<br />
&nbsp;&nbsp; Suggestion about company can be sent from company`s page. The option is located below categories.<br />
 <br />
&nbsp;&nbsp; These suggestions will also be referenced as edit suggestions.<br />
 <br />
&nbsp;&nbsp; User can see the suggestions he can edit/manage in the “Edit suggestions” page. <br />
   <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "> <span style="font-size: 13pt; color: #009933; ">Editor options about edit suggestion</span></p> <br />
  Answer : He can reply to the user which sent suggestion and ask him for something.<br />
  Accept : Indicates that he agrees with the suggestion, and will update the information accordingly.<br />
  <br />
  <span style="font-style: italic; ">Note : After limited time if the editor hasn`t updated the information, the user which sent the suggestion can send report to the support. </span><br />
  <br />
  Decline : Indicates that he doesn`t agree with the suggestion and won`t update the information.<br />
  <br />
<i><span style="font-style: italic; ">Note : The editor must accept or decline the suggestion in limited time (like 3 days). When this time passes, the suggestion will be declined automatically by the system and it`s author will be notified with system message.</span><br />
 <br />
 <span style="font-style: italic; ">Note : If user is suggesting an update to variants, categories or other which is important, in order users to be able to interact properly, then he can send report to the support, if the suggestion is declined.<br />
</span></i>  <br />
  Delete : Will delete the suggestion and set it as ‘Declined’ to the user which sent it.<br />
  <br />
&nbsp;&nbsp; When editor receive edit suggestion or user receive reply for edit suggestion, an icon will show up near the log out link, indicating that there is new information regarding the edit suggestions. That way users will be notified when there is new information, and they won`t have to check the page from time to time.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "> <span style="font-size: 13pt; color: #009933; ">Notes</span></p> <br />
1. If an editor is intentionally not updating information, and an update is necessary, please send report to the support after he declines the suggestion.<br />
 2. If suggestion is not sent from edit suggestion form, then it won`t be classified as such to the support, and it won`t be inspected if report is sent. <br />
 3. Editors also have the option to sent reports about edit suggestions.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infts')
,(N'Information about system messages', N'&nbsp;&nbsp; In some cases support or users make action which in some way is related to some user. Because the user needs to be informed in some way, system sends message to him. They are sent automatically in certain situations.<br />
 <br />
 <span style="font-style: italic; ">Example : If an administrator removes a user right, the user will receive a system message telling him that.</span><br />
 <br />
&nbsp;&nbsp; System messages can be seen in user`s&nbsp;“Profile”&nbsp;page. <br />
 <br />
&nbsp;&nbsp; When user receives system message, an icon will appear near the log out link, notifying him that he received such.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infsm')
,(N'Information for warnings', N'&nbsp;&nbsp; Warnings are the public way in which users will be sanctioned. When support decides that user broke rules either by reports or other way, warning is one of the ways that he can be sanctioned.<br />
 <br />
&nbsp;&nbsp; Warnings will be shown in user`s “Profile” page. They can be seen by all users.<br />
 <br />
&nbsp;&nbsp; Have in mind that after reaching a number of warnings (2-5) about same thing, the system will automatically remove that user right. &nbsp;Also when user receives definite number of warnings his account will be deleted.<br />
 <br />
 <span style="font-style: italic; ">Example : If user receives 2 warnings about breaking the comment posting rules, on the third warning the system will remove his right to post comments.</span><br />
 <br />
 <span style="font-style: italic; ">Example : If user receives total of 10 warnings, his account will be automatically deleted by the system.</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infwarns')
,(N'Information for notifications', N'&nbsp;&nbsp; Users can receive notifications when new content in products/companies/forums/topics&nbsp;is added.&nbsp;Notifications help avoiding the necessity for the users to periodically check if there is new information, in which they are interested.<br />
 <br />
&nbsp;&nbsp; User can manage his notifications from “Notifications” page in the “Profile” menu.<br />
 <br />
&nbsp;&nbsp; If users want to receive notifications when there is new content about products/companies/forums/topics, they should visit their pages and click the notify button. &nbsp;<br />
 <br />
&nbsp;&nbsp; Currently users will receive notifications about product when there are new opinions for it.<br />
&nbsp;&nbsp; Users will receive notifications about a company when new products are added to it.<br />
&nbsp;&nbsp;&nbsp;Notifications about forum will be received when new topics are started in it.<br />
&nbsp;&nbsp; Notifications about topic will be received when new comments are posted in it. &nbsp;<br />
<br />
&nbsp;&nbsp;&nbsp;When there is new content for product/company/forum/topic, an icon will show near the log out link, indicating that.&nbsp;In the “Notifications” page users can see where exactly the new content is. From there users can also unsubscribe from notifications.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infnotifs')
,(N'General information for edit rights and right transfers', N'&nbsp;&nbsp; When a user adds a product/company, he becomes its editor. Editor means that he can modify it. Besides that, he also takes the responsibility to update it if necessary while he has the rights to do so.<br />
 <br />
&nbsp;&nbsp; Users can remove their editor rights if they don`t want them anymore. This can be done from edit rights page, which can be accessed from profile menu.<br />
 <br />
&nbsp;&nbsp; User`s edit rights can be seen by everyone, by visiting his profile page and clicking on ‘Edit rights’ link.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Transfer rights</span></p><br />
&nbsp;&nbsp; In edit rights page, there is option for transfer of almost each right. Transfer right option gives the possibility to give the right to another user, in case the current editor doesn’t want it anymore.<br />
<br />
<span style="color: #009900"><font color="#404040">&nbsp;&nbsp; </font></span><span style="color: #009933; ">Notes:</span><br />
1. When a right is being transferred to a user, he has limited time to accept or decline it. After the limited time passes the transfer will be declined by the system automatically.<br />
 2. While a right is in process of transfer, its current owner can still use it.<br />
 3. When user accepts a transfer, he becomes the new editor and the old editor gets his right removed.<br />
 4. While right is being transferred, the editor has the option to cancel it, if he changes his mind.<br />
 5. Editor cannot transfer rights to users he blocked, and user cannot receive transfer of rights from users he blocked.<br />
 <br />
 <span style="font-style: italic; text-decoration: underline; ">Important : Since editors are responsible to update the information for companies/products on which they have rights, there is time, for which if editor doesn''t log in, other users will be able to take his edit rights.</span><br />
 <br />
 <span style="font-style: italic; ">Example : If editor doesn''t log even once for more than 3 months, other users will be able to take all his edit rights, and thus becoming new editors.</span>&nbsp;<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; color:#c02e29; "><u>Editor rights can be given and removed by site support at any time.</u></span></p>', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'ginfrt')
,(N'General information', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29;"><u>Site team is not responsibl</u><u>e for the information, posted and shared by the users!</u></span></p>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29;"><u><br />
</u></span></p>
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Site support can punish users, which are not following the rules with the following methods</span></p><br />
1. Send to the user a warning, which can be seen by other users.<br />
 2. Remove right. For example, if the user repeatedly breaks opinion posting rules, the support can remove his right to post opinions.<br />
 3. Delete account. If user breaks rules continuously, his account can be deleted without notice.<br />
 4. Ban IP address. If users keep breaking rules from an IP address, that address can be banned. Users won`t be able to register, log in, post comments from banned IP address.<br />
 <br />
 <span style="font-style: italic; text-decoration: underline; ">Important : There is no strict order to follow with punishments, that said accounts can be deleted or rights removed without warnings. If user considers that he is being punished unjustly, he should contact site support via report or “About” page form.</span><br />
 <br />
&nbsp;&nbsp; This is done in order to prevent to the possible extent the users from taking harmful actions.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">General restrictions which users should be aware of</span></p><br />
1. Guests will have to wait limited time between posting opinions.<br />
 2. Newly registered users will have to wait also time between posting opinions until their opinions count reaches a certain number.<br />
 3. Number of registrations from one email address is limited.<br />
 4. Number of registrations from one IP address is limited.<br />
 5. Number of opinions per product is limited for registered users and guests. <br />
 <br />
 <span style="font-style: italic; ">Example : 2 comments per IP address can be posted for each product.</span><br />
 <br />
 6. Guests can`t reply to opinions. <br />
 7. The number of suggestions that users can post for site is limited.<br />
 <span style="color: black; ">8. After a user adds a company, product or topic, he will have to wait certain time before adding the next.<br />
</span>9. For every product/topic, up to certain number of opinions/comments can be rated by registered user.<br />
 10. Registered users can rate certain number of opinions each day.<br />', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulgi')
,(N'aboutReportUser', N'Please report only about the visited user.<br />
 <br />
 Report for a user should be about rules he breaks.<br />
 Describe exactly where the user broke rules so we can check and react accordingly.<br />
<br />
  More information can be read <a shape="rect" href="Guide.aspx#infabreports" target="_blank">here</a>.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'abru')
,(N'aboutWarnings', N'&nbsp;&nbsp; When user breaks a rule, send him warning before anything else. If a role needs to be revoked, send warning to user and after that, revoke his role. Describe why the user is receiving it, because everyone can see his warnings. <br />
&nbsp;&nbsp; Use ''general'' warning, only when it`s not connected with any of the roles.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutwarnings')
,(N'aboutEditSuggestions', N'<p style="text-align: center; margin: 0px"><span style="font-family: times new roman,times,serif; color: #003399; font-size: 16pt">Here you can manage the edit suggestions which you sent and/or received.</span></p><span style="font-style: italic"><br />
Note : If you accept suggestion, you have to update the info accordingly.</span><br />
<p style="text-align: right; margin: 0px">More information can be read <a shape="rect" href="Guide.aspx#infts" target="_blank">here</a>.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutes')
,(N'aboutReportEditSuggestion', N'Please report problems only about the selected suggestion.<br />
<br />
&nbsp;&nbsp; The report can be about the following:<br />
 1. Editor declines to update vital information about product/company.<br />
 2. User is spamming or writing suggestions which are not about product/company.<br />
 3. Editor or user is using offensive language.<br />
<br />
  More information can be read <a shape="rect" href="Guide.aspx#infabreports" target="_blank">here</a>.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutres')
,(N'Rules for adding/editing product', N'<ul style="padding-left:30px;">
<li>It is prohibited to fill incorrect and/or offensive information about the product;&nbsp;</li>
<li>Characteristic information must be related to the product;</li>
<li>Advertisements are not allowed;&nbsp;</li>
<li>It is mandatory to add product variants and/or sub-variants if the product has such, in order users to be able to post opinions about them;&nbsp;</li>
<li>Posting of copyrighted information is not allowed!</li></ul> &nbsp;&nbsp; Failure to follow the rules will lead to sanctions.<br />
<br />
<p style="margin: 0px; text-align: center; font-weight: bold; "><u>Editor rights can be given and removed by site support at any time.</u></p><br />
   &nbsp;&nbsp; Information for adding/editing products can be <a shape="rect" href="Guide.aspx#infaeproducts" target="_blank">read here</a>.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29;"><u>Site team is not responsib</u><u>le for the information, posted and shared by the users!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesaeprod')
,(N'Rules for adding/editing company', N'<ul style="padding-left:30px;">
<li>It is prohibited to fill incorrect and/or offensive information about the company;&nbsp;</li>
<li>Advertisements are not allowed;</li>
<li>It is necessary and important that all categories in which the company can have products to be added (to the company), in order users to be able to add products to the company;&nbsp;</li>
<li>Posting of copyrighted information is not allowed!</li></ul> &nbsp;&nbsp; Failure to follow the rules will lead to sanctions.<br />
&nbsp;&nbsp; &nbsp;&nbsp;<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; text-decoration: underline; ">Editor rights can be given and removed by site support at any time.</span></p><br />
   &nbsp; &nbsp;Information for adding/editing companies can be <a shape="rect" href="Guide.aspx#infaaemaker" target="_blank">read here</a>.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-size: 12pt; color: #c02e29; "><b><u>Site team is not re</u></b><b><u>sponsible for the information, posted and shared by the users!</u></b></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesaemaker')
,(N'Information for reports', N'&nbsp;&nbsp; Reports are the instrument with which you can let the support know about problems. Problem in this case means : site bug; incorrect information; problem in page; something which does not follow the rules. <br />
 <br />
&nbsp;&nbsp; User needs to be registered and logged in to write a report.<br />
 <br />
&nbsp;&nbsp; Reports can be written from couple of places:
<ul style="padding-left:30px;">
<li>Category page - Users should only report irregularities related with the chosen category page;</li>
<li>Company page - Users should only report irregularities related with the chosen company page;</li>
<li>Product page – Users should only report irregularities related with the chosen product page;</li>
<li>MyReports – From this page users can post general reports, not limited to a specific page;</li>
<li>User page – When user is visiting another user`s page, from there he can fill report about that user.</li>
<li>Topic''s page – Users should only report irregularities related with the visited topic.<br />
</li></ul> <span style="font-style: italic; ">Example : report written from Audi company page is about Audi`s company page.</span><br />
 <br />
&nbsp;&nbsp;&nbsp;There are two types of reports, irregularity and violation. These mentioned above are all irregularity. Violation reports are about user opinions, suggestions and comments. If an opinions/suggestion/comment is marked as a violation, a moderator can delete it.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009933; font-size: 13pt; ">Tips when writing report</span></p>
<ul style="padding-left:30px;">
<li>If the report is being written from “MyReports” page, it is obligatory to mention the page in which the problem is;</li>
<li>Describe your actions before the problem showed as accurately as possible, if it`s a bug or a technical issue;</li>
<li>When writing report about user, please be sure that he has broken a rule.</li></ul>
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">After report is written</span></p>&nbsp;&nbsp;<br />
&nbsp;&nbsp; When the report is written, the support will be able to see it, and work on it. Meanwhile the report will be shown in “MyReports” page. From there the author will be able to follow it`s progress. There will be also couple of options:<br />
&nbsp;&nbsp; &nbsp;Not resolved/resolved text : This text shows if the work on the report is finished. Not resolved means no, resolved means yes;<br />
&nbsp;&nbsp; &nbsp;Delete button : Pressing that button will delete the report;<br />
 <br />
 <span style="font-style: italic; ">Note : If report, which is not resolved get deleted, it will automatically be marked as “resolved” to support and they will stop work on it.</span><br />
 <br />
&nbsp;&nbsp; &nbsp;Reply button : If some kind of information is needed, the support will send message to the user. The message will show below the report, and a Reply button will be available to the user. When you press this button, you will be able to send feedback to the administrators about the report;<br />
 <br />
 <span style="font-style: italic; ">Note : If the report is resolved or support didn’t send any message, the button will be hidden.</span><br />
 <br />
&nbsp;&nbsp; Resolve button : Pressing this button will set the report as “resolved” and the support will stop work on it. It`ll also remain in the user`s reports.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Other information</span></p>
<ul style="padding-left:30px;">
<li>Time in which report will be resolved/answered is not defined;</li>
<li>Every user can have limited maximum amount of not resolved violation and irregularity reports at any time;</li>
<li>Violation reports don`t show in user`s reports section.</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-size: 13pt; color: #009933; ">Rules</span></p>
<ul style="padding-left:30px;">
<li>Reports shouldn`t contain information which is not about irregularities;</li>
<li>Offensive language won`t be tolerated.</li></ul>&nbsp;&nbsp; Failure to follow the rules will lead to sanctions.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infabreports')
,(N'The die is cast – “wiadvice.com” starts!', N'&nbsp;&nbsp; Dear users, after testing we are starting “wiadvice.com”!<br />
 <br />
&nbsp;&nbsp; It took us a little bit more time, but we managed to make visual and functional upgrades. We hope that they will make your visits more fulfilling.<br />
 <br />
&nbsp;&nbsp; Product and company replenishment starts from now on – but as you know this depends mostly on you! Now you have the possibility to show in which of them you are interested and express your opinions. Add at will!<br />
 <br />
&nbsp;&nbsp; Soon we will start adding new visual and functional improvements – some of them will be thanks to <a href="SuggestionsForSite.aspx" target="_blank">your suggestions</a> for the site.<br />
<br />
&nbsp;&nbsp; We await you...', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsSiteStarted')
,(N'Forum added for every product!', N'&nbsp;&nbsp;&nbsp;Friends, now you have the possibility to discuss every product in its own forum. You can do that by clicking the&nbsp;&quot;Forum&quot; link in product''s page, which is near the &quot;Editors&quot; link.', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsForumIntroduced')
,(N'Rules for adding topic/comment', N'<ul>
<li>Topics not related to the respective product are not allowed;</li>
<li>Comments need to be about the topic;</li>
<li>Offensive language is strictly forbidden;</li>
<li>The only comments language should be English;</li>
<li>User attacks in comments are not allowed;</li></ul>&nbsp;&nbsp; <span style="color: #339900; ">Notes :</span><br />
1. After a user adds a topic, he will have to wait certain time before adding the next topic.<br />
2. Newly registered users will have to wait limited time before posting next comment. This is done to slow down&nbsp;possible spam attacks. After users reach certain number of comments, that time restriction will be removed.&nbsp;<br />
3. Users should mark as violation every comment, which isn`t following the rules, in order to keep the comments clear&nbsp;from unnecessary or unrelated information.&nbsp;<br />', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesTopic')
,(N'aboutTopicReporting', N'Please only report problems about the visited topic.<br />
<br />
&nbsp; The report can be about the following:<br />
1. The topic is not connected with the product.<br />
2. Author is using offensive language.<br />
3. The author is advertising product.<br />
4. About technical problem (bug) in the page.<br />
<br />
Please don`t report violations in the comments - mark them as &quot;violation&quot;!<br />
<br />
More information can be read <a href="Guide.aspx#infabreports" target="_blank">here</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'textAboutTopicReporting')
,(N'Option to add links to other sites.', N'&nbsp;&nbsp; From today you have the option to add links to other sites, which contain concrete information for every product. You can&nbsp;find the option near &quot;Forum&quot; and &quot;Editors&quot; in product''s page.<br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsAddLinks')
,(N'Rules for adding product links', N'&nbsp;&nbsp; In order to accelerate finding of information about the wanted product, we added option, which allows you to add links to the&nbsp;product. These links should contain specific information about the product (like technical data/forum/opinions/reviews etc...).<br />
<ul>
<li>It is forbidden the link to point at site, which :<br />
&nbsp;&nbsp; -&nbsp;advertise the product;<br />
&nbsp;&nbsp;&nbsp;- duplicates the information, which can be found in another added link;<br />
&nbsp;&nbsp;&nbsp;- is selling the product (market);<br />
&nbsp;&nbsp;&nbsp;- is not connected with the product.</li></ul>&nbsp;&nbsp; <span style="color: #339900; ">Notes :<br />
</span>
<ul>
<li>Links can only be added by users registered with e-mail;</li>
<li>The links can be modified by the editors and administrators;</li>
<li>Please, mark as &quot;violation&quot; the links, which break the rules;</li>
<li>After a user adds a link, he will have to wait certain time before adding the next;</li>
<li>Please, don`t abuse the option! It`s there to help the users;</li>
<li>The links, which don`t follow the rules will be removed!</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#C02E29" size="3"><b><u>Site team is not responsible for t</u></b></font><font color="#C02E29" size="3"><b><u>he information, posted and shared by the users!</u></b></font></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesProdLinks')
,(N'Minor updates.', N'&nbsp;&nbsp; Hello! An option to change product''s name/category/company is available for editors, up to 30 minutes after the product is added. Same for new&nbsp;company''s name. The option is added in case the user makes an error when adding new product/company and wants to fix it.<br />
<br />
&nbsp;&nbsp; Updates on the site vision are made in order to keep the good look when browsing with Internet Explorer 9 (unnoticeable if you use another browser).<br />
<br />
&nbsp;&nbsp; Recently new versions were released on the most popular browsers (Chrome, Firefox, Internet Explorer...), which have improvements and support new&nbsp;technologies. Because of that, we suggest you to update your browser to it''s latest version, if you have not done so yet. This`ll assure you see sites&nbsp;the way they are designed.<br />
<br />
&nbsp;&nbsp; Links, from which you can download the latest versions :&nbsp;<br />
&nbsp;&nbsp; &nbsp; &nbsp;- Mozilla Firefox : <a href="http://www.mozilla.com/en-US/firefox/new/" target="_blank">http://www.mozilla.com/en-US/firefox/new/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp;- Google Chrome : <a href="http://www.google.com/chrome/" target="_blank">http://www.google.com/chrome/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp;- Safari : <a href="http://www.apple.com/safari/" target="_blank">http://www.apple.com/safari/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp;- Opera : <a href="http://www.opera.com/" target="_blank">http://www.opera.com/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp;- Internet Explorer : <a href="http://windows.microsoft.com/en-US/internet-explorer/products/ie/home" target="_blank">http://windows.microsoft.com/en-US/internet-explorer/products/ie/home</a><br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'minorUpdates1')
GO