USE [master]
GO
/****** Object:  Database [wiadvice_siteDB_BG]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE DATABASE [wiadvice_siteDB_BG]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'wiadvice_siteDB_BG', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_siteDB_BG.mdf' , SIZE = 10432KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'wiadvice_siteDB_BG_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_siteDB_BG.ldf' , SIZE = 5824KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET COMPATIBILITY_LEVEL = 90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [wiadvice_siteDB_BG].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ARITHABORT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET  DISABLE_BROKER 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET  MULTI_USER 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET DB_CHAINING OFF 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [wiadvice_siteDB_BG] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [wiadvice_siteDB_BG]
GO
/****** Object:  User [wiadvice_BgAdmin]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE USER [wiadvice_BgAdmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[wiadvice_BgAdmin]
GO
ALTER ROLE [db_owner] ADD MEMBER [wiadvice_BgAdmin]
GO
/****** Object:  Schema [wiadvice_BgAdmin]    Script Date: 24.1.2015 г. 17:27:37 ******/
CREATE SCHEMA [wiadvice_BgAdmin]
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
			WHERE [name] = 'друга' AND [createdBy] = @systemUserID)
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
ALTER DATABASE [wiadvice_siteDB_BG] SET  READ_WRITE 
GO

USE [wiadvice_siteDB_BG]
GO

INSERT INTO [dbo].[UserIDs] ([ID],[haveNewContent])
       VALUES (1 , 0) -- system
	   , (2 , 0) -- guest
	   , (3 , 0) -- admin 
GO

INSERT INTO [dbo].[CompanyTypes] ([name],[visible],[createdBy],[dateCreated],[lastModifiedBy],[lastModified],[description])
     VALUES ('other',1,1,GETUTCDATE(),1,GETUTCDATE(),'If other types don`t suit choose this one.')
	 , ('company',1,3,GETUTCDATE(),3,GETUTCDATE(),'For all companies.')
GO

INSERT [dbo].[Companies] ([type], [name], [visible], [dateCreated], [description], [createdBy], [lastModifiedBy], [lastModified], [website], [canUserTakeRoleIfNoEditors]) 
	VALUES (2, N'друга', 1, GETUTCDATE(), N'All products who are from companies that are not added will be from this..', 1, 1, GETUTCDATE(), NULL, 1)
GO

SET IDENTITY_INSERT [dbo].[Categories] ON 
GO
INSERT [dbo].[Categories] ([ID], [parentID], [name], [dateCreated], [last], [description], [createdBy], [lastModifiedBy], [lastModified], [visible], [imageUrl], [imageWidth], [imageHeight], [displayOrder]) 
VALUES 
(1, 17, N'Превозни средства', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(3, 1, N'Автомобили', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(5, 1, N'Мотоциклети', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(6, 1, N'Летателни апарати', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 8)
,(7, 1, N'Лодки', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(8, 17, N'Електроника', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(9, 8, N'Компютри', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(10, 330, N'GSM', GETUTCDATE(), 0, N'', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(11, 66, N'TV', GETUTCDATE(), 1, N'Рекламно-информационен и спортно-музикален сандък, облъчващ с до 200 програми (от които реално използваме има-няма 5), и със сериали до 5000 епизода. :)', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(15, 10, N'Стандартни', GETUTCDATE(), 1, N'Апаратчето, което държите в ръка, когато звъните на мама, татко или приятел, и което изключвате, ако не искате шефът да се свърже с вас! :)', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(16, 10, N'Смартфони', GETUTCDATE(), 1, N'Притежават функциите на GSM, съчетани с някои възможности и на нетбук – операционна система, интернет връзка, работа с офис програми и други. И ще бъдете в час какво става в социалните мрежи! :)', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(17, NULL, N'Категории', GETUTCDATE(), 0, N'root', 1, 1, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(18, 5, N'Мотоциклети', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(19, 18, N'Стандартни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(20, 9, N'Лаптопи', GETUTCDATE(), 1, N'„Мобилност преди всичко!”, но съчетана с възможностите на настолния компютър. Автономния режим на захранване позволява да се използва навсякъде и по всяко време.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(21, 6, N'Самолети', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(22, 7, N'Моторни лодки', GETUTCDATE(), 1, N'За работа, спорт и развлечение… :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(27, 18, N'Спортни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(28, 18, N'Оффроуд', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(32, 3, N'Спортни', GETUTCDATE(), 1, N'„Колкото повече, толкова повече!” – цифрите винаги са от значение, но не забравяйте да затегнете коланите! :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(33, 3, N'Леки', GETUTCDATE(), 1, N'От малки всички мечтаем за тях! :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(34, 3, N'Лимузини', GETUTCDATE(), 1, N'Много лукс за много пари – чак е излишно…! :) Но ако за вас да покажете статус е от значение – тук са вашите играчки!', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(37, 1, N'Камиони', GETUTCDATE(), 1, N'Пренасят 90+% от предметите в категориите на сайта! :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(38, 1, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви превозни средства, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(39, 1, N'Лекотоварни', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(40, 39, N'Мини ванове', GETUTCDATE(), 1, N'Подобни са по форма на вановете, но с по-ниско шаси и по-малък размер. Предназначени са за превоз на хора и стоки.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(41, 393, N'SUV', GETUTCDATE(), 1, N'В България всички тях ги наричаме „Джипове”, но всъщност те са купе на голям седан или комби, качено на по-високо шаси. За разлика от истинските джипове (оффроудите), SUV-овете са с по-слабо окачване, не всички са 4x4 и се движат предимно по първокласни и второкласни пътища. При производството им не се набляга на оффроуд възможностите. Типични представители на „SUV-овете” са например: Audi Q7; BMW x5/x6; Mercedes-Benz M-Class; Honda CR-V.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(42, 39, N'Пикапи', GETUTCDATE(), 1, N'Автомобили с открита или закрита каросерия, предназначени за превоз на различни товари. Характерни са с разделението на пътническата кабина от товарната площ.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(43, 39, N'Ванове', GETUTCDATE(), 1, N'В България са известни като „бусове”, но по света се наричат „ванове” – предназначени са за превоз на стоки и материали, шасито им е като на голям автомобил (но по-високо) и товарната площ е закрита.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(44, 5, N'ATV', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(45, 6, N'Хеликоптери', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(46, 6, N'Балони', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(47, 6, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви летателни апарати, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(48, 7, N'Яхти', GETUTCDATE(), 0, N'to be added', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(49, 7, N'Спортни и скоростни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(50, 9, N'Настолни', GETUTCDATE(), 1, N'Е, шансът да използвате в този момент такъв предмет е 50/50. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(52, 9, N'Сървъри', GETUTCDATE(), 1, N'Устройства, чрез които се събира или „разпределя” информация към локални или интернет мрежи.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(53, 9, N'Други компютри', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви продукти, свързани с компютри, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(55, 65, N'MP3 плейъри', GETUTCDATE(), 1, N'Всеки от нас има любима музика или изпълнители – Mp3 плейърите са едно от устройствата, чрез които им се наслаждаваме, защото Mp3 формата е най-разпространеният в момента за запис на музика и аудио.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(56, 8, N'Камери', GETUTCDATE(), 0, N'to be added.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(57, 56, N'Фотоапарати', GETUTCDATE(), 1, N'При различни поводи и събития, за снимане на природата и въобще света около нас, все още фотоапаратите са предпочитани заради качеството на изображението и високата разделителна способност.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(63, 56, N'Охранителни камери', GETUTCDATE(), 1, N'За гарантиране на вашата сигурност, за наблюдение на обекти и зони, се използват охранителни камери, различаващи се според предназначението и технически параметри.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(64, 56, N'Камери', GETUTCDATE(), 1, N'Записи на любими моменти, незабравими мигове и кадри от мястото на събитията правиме с различни камери. Професионални, полупрофесионални или любителски – тяхното място е тук.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(65, 8, N'Аудио', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(66, 8, N'Видео', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(67, 8, N'Друга електроника', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви продукти с електроника, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(69, 17, N'Музика', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(72, 9, N'Хардуер', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(78, 79, N'Скенери', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакъв външен хардуер, който не отговаря на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(79, 72, N'Периферия', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(80, 72, N'Вътрешен', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(81, 79, N'Принтери', GETUTCDATE(), 1, N'Устройство за отпечатване на текст и изображения (представени в цифров вид в компютъра) върху хартиен носител.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(82, 79, N'Монитори', GETUTCDATE(), 1, N'Шансът в момента да гледате такъв (под една или друга форма) е 99.999%… :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(84, 79, N'Игрална', GETUTCDATE(), 1, N'„Екстрите”, които доближават преживяването пред компютъра по-близко до реалността :) – волани, ръчки, джойстици и други. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(85, 79, N'Други', GETUTCDATE(), 1, N'Тук можете да добавяте и намирате всякакъв външен хардуер, който не отговаря на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(86, 79, N'Клавиатури', GETUTCDATE(), 1, N'Терминал за управление на сложни електронни схеми. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(87, 79, N'Мишки', GETUTCDATE(), 1, N'Малко са начините, по които можете да достигнете до тук, без такава. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(88, 80, N'Видео карти', GETUTCDATE(), 1, N'За да работят пълноценно най-новите игри и програми за обработка на графика, главното от което се нуждаете е мощна видео карта.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(89, 80, N'Твърди дискове', GETUTCDATE(), 1, N'Устройство за четене и запис на данни. Ако искате да държите повече информация на компютъра, без да се налага да триете често, се намирате в правилната категория. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(90, 80, N'RAM памети', GETUTCDATE(), 1, N'Ако искате вашият компютър да има по-голяма производителност, това може би е едно от нещата, което ще трябва да обновите.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(91, 80, N'CD roms', GETUTCDATE(), 1, N'Всеки домашен компютър се нуждае от устройство за четене и запис на CD, DVD, Blu-ray, и други дискове. Различните видове ще откриете тук.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(92, 80, N'Процесори', GETUTCDATE(), 1, N'От тях основно зависят изчислителните възможности на компютрите. Колкото по-добър е процесорът, толкова по-бърз ще е компютърът и програмите, с които работите. ', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(93, 80, N'Дънни платки', GETUTCDATE(), 1, N'Дънните платки определят каква може да е останалата част от конфигурацията на компютъра, както и възможностите за обновяване, затова ги избирайте внимателно.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(95, 80, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакъв вътрешен хардуер, който не отговаря на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(99, 79, N'Мрежова периферия', GETUTCDATE(), 1, N'Рутери, хъбове, модеми и всякакви външни устройства, свързани с осигуряването на достъп до мрежата, търсете тук.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(100, 359, N'Бинокли', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(101, 17, N'Техника', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(102, 101, N'Битова', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(103, 102, N'Перални', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(104, 102, N'Хладилници', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(105, 102, N'Готварски печки', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(106, 102, N'Бойлери', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(111, 102, N'Кафе машини', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(129, 101, N'Климатици', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(141, 17, N'Обзавеждане', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(144, 141, N'Столове', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(160, 144, N'Обикновени', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(161, 144, N'Офис', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(163, 144, N'Други', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(186, 330, N'GPS', GETUTCDATE(), 1, N'Система, която позволява да се ориентирате в сложната градска среда или да получавате информация за маршрутите. Това устройство дава възможност и за локализиране на различни обекти.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(322, 3, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви автомобили, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(324, 8, N'Игрални конзоли', GETUTCDATE(), 1, N'От 5 до 55г. всички обичаме да играем на електронни игри – игралните конзоли ни дават възможност да се наслаждаваме в свободното време пред екрана.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 8)
,(325, 65, N'Тонколони', GETUTCDATE(), 1, N'Качествена музика и качествен звук – мечтата на всеки музикален фен – тонколоните у дома или в любимата дискотека ни пренасят в мечтите.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(326, 66, N'Проектори', GETUTCDATE(), 1, N'За учебни и бизнес цели, за презентации и реклама, проекторите са съвременния избор заради своята мобилност и мащабност на изображението.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(330, 8, N'Комуникация', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(331, 330, N'Телефони', GETUTCDATE(), 1, N'Вкъщи, в офиса или в търговските обекти, стационарните телефони си остават важна част от комуникацията помежду ни.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(332, 330, N'Факс машини', GETUTCDATE(), 1, N'Неделима част от съвременните бизнес отношения е изпращането и получаването на всякакви документи. Факс-апарата съчетава в себе си телефон, принтер и различни екстри в зависимост от модела. За връзка използва телефонната мрежа.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(333, 66, N'Домашни видео системи', GETUTCDATE(), 1, N'Концерт на живо, филмова премиера у дома или мач на любимия отбор – пълно усещане за това, което се случва, ви представят домашните видео системи, чрез съчетанието на аудио и видео формати.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(335, 65, N'Аксесоари', GETUTCDATE(), 1, N'Слушалки, микрофони и др. продукти, необходими и/или допълващи удоволствието ни от музиката или друга аудио информация.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(336, 66, N'Плейъри', GETUTCDATE(), 1, N'Ако искате да гледате интересни филми и концерти, да си припомните любимите мигове и моменти, записани на DVD, Blu-ray или други носители, плейърите ви дават тази възможност.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(337, 65, N'Мини системи', GETUTCDATE(), 1, N'Какво по-хубаво от това да сменяте дискове с различна музика с едно натискане на дистанционното – мини-системите събират в едно радио-касетофон, CD-чейнджър, тонколони и др. екстри, и доставят удоволствие на всеки със своята функционалност.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(338, 8, N'Електроника за автомобили', GETUTCDATE(), 1, N'Ако вие и вашият автомобил се нуждаете от удобство, функционалност и сигурност, тук са продуктите, които ще ги осигурят – MP3, DVD, тонколони и суббуфери, аларми, камери и др.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(339, 9, N'Нетбуци', GETUTCDATE(), 1, N'Ами това е „по-малкото братче” на лаптопа – с по-малък размер и по-малки възможности. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(340, 9, N'Таблети', GETUTCDATE(), 1, N'Малки преносим компютри, снабдени със сензорен екран, чрез който се манипулира. Предназначени са предимно за сърфиране в интернет и четене на книги, но разполагат с възможностите, които предлагат операционните системи.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(341, 17, N'Строителство', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(342, 341, N'Строителни машини', GETUTCDATE(), 1, N'Багери, фадроми, бетоновози, електрокари и мотокари, влекачи, валяци и въобще всякакъв вид и размер строителна техника.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(343, 341, N'Строителни материали', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(344, 341, N'Ел. инструменти', GETUTCDATE(), 1, N'Желани играчки за пораснали момчета! :) Професионални или не, те са безценен помощник за всеки.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(345, 341, N'Ръчни инструменти', GETUTCDATE(), 1, N'Основни предмети, без които не можем в ежедневието. Но за съжаление, не всички умеят да ги използват… :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(346, 341, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате предмети и продукти в строителството, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(347, 341, N'Сглобяеми конструкции', GETUTCDATE(), 1, N'Конструктор „Лего”, но в по-големи мащаби. :) Използването им спестява време и средства, монтажът им е лесен и в зависимост от материала и типа на изработване осигуряват различна топло и звуко-изолация.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(348, 343, N'Сухи строителни смеси', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(349, 343, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате строителни материали, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(350, 343, N'Бои и лакове', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(351, 343, N'Грундове и мазилки', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(352, 343, N'Баня и санитария', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(353, 101, N'Селскостопанска', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(354, 101, N'Градинска', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(355, 353, N'Комбайни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(356, 353, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате селскостопанска техника, която не отговаря на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(357, 353, N'Трактори', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(358, 102, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате битова техника, която не отговаря на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(359, 17, N'Други', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(360, 359, N'Очила', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(361, 359, N'Часовници', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(362, 141, N'Баня', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашата баня.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(363, 141, N'Спалня', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашата спалня.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(364, 141, N'Детска стая', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашата детска стая.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(365, 141, N'Кухня', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашата кухня.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(366, 141, N'Хол', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашия хол.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(367, 141, N'Офис', GETUTCDATE(), 1, N'Тук може да добавите продукти или да намерите информация, свързанa с обзавеждането на вашия офис.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 6)
,(368, 141, N'Други', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(369, 17, N'Спорт', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(370, 69, N'Аксесоари и други', GETUTCDATE(), 1, N'Съвременната музика, концертната, театралната и филмовата дейности не могат без много допълнения – професионални микрофони, струни, осветителна техника, шоу-електроника и др. Тук ще ги намерите.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(371, 69, N'Инструменти', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(373, 69, N'Звукова техника', GETUTCDATE(), 1, N'Продуктите в тази категория са за професионалисти и познавачи, защото се използват в определени случаи – концерти, театрална и филмова дейност, масови мероприятия и др. – смесителни пултове, DJ – оборудване, професионална и полупрофесионална звукова техника.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(374, 371, N'Струнни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(375, 371, N'Други', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(376, 371, N'Електронни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(377, 371, N'Клавишни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(378, 371, N'Ударни', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(379, 371, N'Духови', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(380, 369, N'Основни принадлежности', GETUTCDATE(), 1, N'Предмети, без които различните спортове не могат да се практикуват – топки за футбол, баскетбол и други, тенис-ракети, стикове, ски, сърфове … - Вече схванахте идеята. :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(381, 369, N'Фитнес уреди', GETUTCDATE(), 1, N'„Здрав дух в здраво тяло!” – У дома или във фитнес залата, всеки има нужда от тях, за да поддържа добра форма!', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 1)
,(382, 369, N'Лов и риболов', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(383, 369, N'Аксесоари и други', GETUTCDATE(), 1, N'Всички останали продукти, които правят спортуването по-приятно и безопасно…', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(384, 382, N'Аксесоари', GETUTCDATE(), 1, N'Тук може да добавите всичко останало за оборудването на ловеца и рибаря.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(385, 382, N'Въдици', GETUTCDATE(), 1, N'Предметът, с който всеки иска да хване златната рибка! :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(386, 382, N'Ловни оръжия', GETUTCDATE(), 1, N'Предимно гладкоцевно огнестрелно оръжие, ловни ножове, арбалети и други.
Забраняваме добавянето на всякакви бойни оръжия или мнения за тях!
', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(387, 7, N'Персонални (Джетове)', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(388, 7, N'Други', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(389, 1, N'Автобуси', GETUTCDATE(), 1, N'Срещат се от време на време по спирки и автогари! :)', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(390, 1, N'Велосипеди', GETUTCDATE(), 1, N'За спорт, удоволствие или придвижване, велосипедът е все още актуален със своята екологичност, мобилност и здравословност. С различно предназначение и качество – тяхното място е тук.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(391, 1, N'Гуми и аксесоари', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(392, 39, N'Бусове', GETUTCDATE(), 1, N'По-малък автобус  с възможност за превоз на около 30 човека. Най-ярък пример са т. нар. „маршрутки”.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(393, 39, N'Джипове', GETUTCDATE(), 0, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(394, 5, N'Мотопеди', GETUTCDATE(), 1, N'Мотопедите обикновено са ограничени до 50 км/ч и максимален обем на двигателя от 49 куб.см.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 2)
,(395, 5, N'Скутери', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 3)
,(397, 65, N'Други', GETUTCDATE(), 1, N'Тук може да добавяте и намирате всякакви продукти, свързани с аудио-техника, които не отговарят на другите категории.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
,(398, 141, N'Подови настилки', GETUTCDATE(), 1, N'За нашият домашен уют и комфорт, за различни помещения и предназначение, всички ползваме килими, мокети, балатуми, ламиниран паркет и др. Тук ще намерите информация за тях.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 9)
,(399, 341, N'Дограми', GETUTCDATE(), 1, N'От материалът, от който е изработена (дърво, PVC или метал), зависи нейната здравина, функционалност и издръжливост. Вътрешна или външна, тя осигурява безопасност и комфорт.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 5)
,(400, 48, N'Моторни яхти', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(401, 48, N'Ветроходни яхти', GETUTCDATE(), 1, N'', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(402, 393, N'Оффроуд', GETUTCDATE(), 1, N'Предназначени са предимно за движение по пресечени и силно пресечени терени, както и по обикновени пътища. Различават се от „SUV-овете” не толкова по външен вид, колкото по технически параметри – по-здраво окачване, задвижване 4x4, по-здрави предавки, големи грайфери на гумите и др. Типични представители са : Lada Niva; Hummer H1; Land Rover Defender; Jeep Wrangler.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 0)
,(403, 369, N'Спортни обувки', GETUTCDATE(), 1, N'Казват, че успехите в спорта зависят от тях! :) Уточнявамe, че става въпрос за всякакъв тип обувки за различните спортове.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 4)
,(404, 8, N'Сигнално-охранителна', GETUTCDATE(), 1, N'Електрониката, която е свързана с безопасност и сигурност на инфраструктурата, частната и държавна собственост – скенери, детектори, датчици, аларми, светлинна сигнализация и други.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 7)
,(405, 359, N'Неопределени', GETUTCDATE(), 1, N'Тук са продуктите, за които редакторите на компании все още не са добавили съответните категории.
Добавянето на продукт в тази категория става само с предварително избрана „компания”.
За повече информация, вижте в наръчника.', 3, 3, GETUTCDATE(), 1, NULL, NULL, NULL, 10)
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO

INSERT [dbo].[SiteNews] ([name], [description], [dateCreated], [type], [visible], [createdBy], [lastModified], [lastModifiedBy], [linkID]) 
VALUES 
(N'about', N'&nbsp;&nbsp; Здравейте! Създадохме този сайт, за да могат потребителите да обменят мнения за продуктите, които купуваме. Сигурни сме, че правите разлика между “говорилня” и коментари – тук няма да обсъждаме, а ще даваме мнения и оценка за продуктите!&nbsp;Новото е, че самите вие можете да добавяте компании и продуктите им, които ви интересуват!<br />
<br />
<p style="text-align: right; margin: 0px">Повече за идеята на сайта - <a shape="rect" href="AboutUs.aspx" target="_blank">тук</a>.</p>
<p style="text-align: right; margin: 0px"><a shape="rect" href="Guide.aspx" target="_blank">Информация</a> за работа със сайта.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'3')
,(N'aboutExtended', N'<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp; Ежедневно ни облъчват с всякакъв вид реклами, които ни убеждават, че „не можем” без този или онзи продукт. Освен това, компаниите не дават гласност на факти, които уронват престижа на тях и продуктите им. А и голяма част от стоките вече не са това, за което се рекламират…! </div> <br />
<div style="background-color:#edf5fe; padding:5px;">&nbsp;&nbsp; <span style="font-style: italic; ">Има много места, на които се дискутират проблемите им, но намирането на актуална информация за качеството на продуктите отнема немалко време, защото е разпръсната. Затова е необходимо място, на което да се събира и открива лесно такава информация.</span></div> <br />
<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp; За да спестим време (и нерви!) информацията, която потребителите ще споделят в сайта, трябва да е само под формата на&nbsp;мнения и коментари – без излишни словоизявления или сравнения с други продукти.</div> <br />
<div style="background-color:#edf5fe; padding:5px;">&nbsp;&nbsp; <span style="font-style: italic; ">Във всеки един момент потребителите имат възможност да добавят компании и продукти, според желанието им, и съответните мнения към тях. Но, както знаете, не трябва да има „права без отговорност”, затова ви уверяваме, че сайтът ще бъде модериран постоянно, и че администраторите стриктно ще следят за спазване на правилата му!&nbsp;И тук ще търсим постоянно вашата помощ за контрол на некоректните потребители – моля, всеки от вас, във всеки един момент, когато забележи нередност или нарушаване на правилата, веднага да сигнализира администраторите на сайта.</span></div> <br />
<div style="background-color:#e7f3e4; padding:5px;">&nbsp;&nbsp; <span style="font-weight: bold; ">Постарахме се да създадем новаторски продукт, интересен с това, че вие го доизграждате, развивате, контролирате, т.е. от вас самите зависи неговия облик. Ще използвате ли възможността?</span></div><br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'4')
,(N'suggestionsAbout', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #003399; font-size: 16pt; font-family: times new roman, times, serif; ">Вашите идеи за развитието на сайта</span></p> <br />
&nbsp;&nbsp;&nbsp;За да давате предложения, трябва да сте регистрирани с ел.поща!<br />
<span style="color: #cc0000; ">&nbsp;&nbsp; Предложенията, които не са относно сайта, ще бъдат премахвани!</span>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'5')
,(N'aboutFAQ', N'&nbsp;&nbsp; Тук ще има информация за често задавани въпроси и отговори.&nbsp; Но ако не откриете това което търсите, проверете в „<a href="Guide.aspx" target="_blank">наръчника</a>” и „<a href="Rules.aspx" target="_blank">правилата</a>”, и не се притеснявайте да ни попитате чрез страницата „<a href="AboutUs.aspx" target="_blank">За сайта</a>”. <br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; ">Въпросите и отговорите ще се попълват във времето.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'6')
,(N'aboutRules', N'&nbsp;&nbsp; Потребителите посещават сайтовете, за да намерят и споделят информация, или просто за забавление. Това важи и за този сайт - затова е необходимо правилата да се спазват, за да може времето, прекарано тук, да е полезно и да е удоволствие за всички! <br />
 <br />
&nbsp;&nbsp;&nbsp;Моля, прочетете и следвайте правилата на сайта, и докладвайте на администраторите тези, които не ги спазват!<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'7')
,(N'Правила и бележки за писането на мнения', N'<ul style="padding-left:30px;">
<li>Мненията задължително трябва да са свързани със съответния продукт!;</li>
<li>Строго забранено е използването на обидни и нецензурни думи!;</li>
<li>Задължително е мненията да са на Български език(кирилица)!;&nbsp;</li>
<li>Забранена е агресия в тона на мненията!;</li>
<li>Ако потребител иска да изрази мнение, което вече е написано от друг, препоръчваме ви просто да оцените вече съществуващото;</li></ul>&nbsp;&nbsp; <span style="color: #009900; ">Бележки :</span><br />
 1. Потребителите могат да пишат ограничен брой мнения за всеки продукт.<br />
 2. Новорегистрираните потребители и „гости” трябва да изчакват определено време между всяко мнение - това е необходимо, за да се забавят възможни „спам” атаки. При наличието на определен брой мнения, това ограничение ще отпадне за регистрираните потребители.<br />
 3. Маркирайте като „нарушение” всяко мнение, което нарушава правилата, за да няма излишна информация.<br />
 <br />
&nbsp;&nbsp; Неспазването на правилата ще доведе до санкции!<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29; "><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulescomms')
,(N'registration', N'<p style="text-align: center; margin: 0px"></p>
<p style="text-align: center; margin: 0px"></p>
<p style="text-align: center; margin: 0px"><span style="font-family: times new roman,times,serif; color: #003399; font-size: 16pt">При регистриране ще получите следните възможности :</span></p><br />
1.&nbsp;Участие във форума на всеки продукт в сайта (започване на теми, писане на&nbsp;коментари).<br />
2. Изпращане на лични съобщения към други потребители.<br />
3. Добавяне на продукти/компании.<br />
4. Оценяване на продукти и мнения.<br />
5. Писане на предложения за сайта.<br />
6. Докладване за нередности и потребители, които не спазват правилата на сайта.<br />
7. Изпращане на предложения относно продукти и компании към техните <span style="font-weight: bold">редактори</span>.<br />
<br />
&nbsp;&nbsp; <span style="color: #c02e29; ">Има два вида регистрации - с и без ел.поща.</span> Ако се регистрирате без ел.поща, няма да имате права за т.: 3, 5, 7.<br />
<br />
&nbsp;&nbsp; Повече информация за регистрацията може да <a shape="rect" href="Guide.aspx#infar" target="_blank">прочетете тук</a>.<br />
<br />
<p style="text-align: center; margin: 0px">С регистрирането приемате <a shape="rect" href="Rules.aspx" target="_blank">правилата</a> за използване на сайта.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'13')
,(N'aboutAdmins', N'&nbsp;&nbsp; You will be able to register administrator only on level below you, exept for global administrators. Select only the needed roles for new administrator, and do not register if there is no need to.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'19')
,(N'aboutAdverts', N'&nbsp;&nbsp; Advertisements are managed from here. Use it to add/edit/delete advertisements. If you are not familiar how to work with advertisements read the Fields information.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009900">Field information</span></p><br />
 1. Html : Type Html code for the advertisement if it is required. Do not forget to include the code for showing the advertisement. If this field has text then the information in ‘Upload File’ and ‘Advert url’ will not have any affect. If html code is not required DO NOT fill this field.<br />
 2. Upload File : If the Advertisement is a multimedia file (image/flash) then select from where it will be uploaded, because the file needs to be on the server.<br />
 3. Advert url : If file is going to be uploaded from ‘Upload File’ field then you need to type to which web site (URL) the user will be redirected when clicked on the advertisement.<br />
 4. Category IDs : Type the IDs for which categories (and their subcategories) the advertisement will be shown. Can be empty.<br />
5. Company IDs : Type the IDs of companies in which the advertisement should show, it will be also shown in these companies products. Can be empty.<br />
 6. Product IDs : Type the IDs of products in which the advertisement should show. Can be empty.<br />
<br />
<span style="font-style: italic">Note : When you edit advertisement, if you want the old links (for categories, companies, products) to be preserved, type them again or they will be deleted.</span><br />
<br />
 7. Expire date : Type the date untill which the advertisement will be active, if there is no such date leave the field empty. <br />
 8. Active : Check it if the advertisement should show in the site`s pages when it`s added. If the it isn`t checked the advertisement will be added to database but it will not show in site`s pages.<br />
 9. General : Check this if the advertisement should show in all pages. If none of the Company/Product/Category ID fields are filled then you need to check General.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'20')
,(N'aboutLogs', N'<p style="text-align: center; margin: 0px">Logs are shown here. Choose criterias from the options and click Show button to show the Logs.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'21')
,(N'aboutAddCompany', N'<ul style="padding-left:30px;">
<li>Когато добавите компания, вие автоматично ставате неин&nbsp;<span style="font-weight: bold; ">редактор</span>. Това ви позволява да я модифицирате;</li>
<li><span style="color: #c02e29; ">След като добавите компания, ще трябва да добавите категории, в които ще могат да се добавят продукти към нея.</span> Това е важно, защото в противен случай, вие и потребителите няма да имате тази възможност. Опцията за добавяне на категории ще бъде налична от страницата на компанията;</li>
<li>След като добавите компанията, ще можете да добавяте изображения/характеристики/... До 30 минути след нейното добавяне може да се променя името и;</li>
<li>Няма да имате възможността да променяте продукти, които са добавени към тази компания, освен ако не сте ги добавили вие.</li></ul>&nbsp;&nbsp; Повече информация за добавяне/промяна на компания може да <a shape="rect" href="Guide.aspx#infaaemaker" target="_blank">прочетете тук</a>.<br />
&nbsp;&nbsp; Правилата за добавяне/промяна на компания могат да бъдат <a shape="rect" href="Rules.aspx#rulesaemaker" target="_blank">прочетени тук</a>.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'22')
,(N'aboutAddProduct', N'&nbsp;&nbsp;&nbsp;<span style="color: #cc0000; ">Важно :</span>&nbsp;Възможно е в някои държави да се продават различни варианти на един и същ модел&nbsp;продукт!&nbsp;Желателно е &quot;Вариантите&quot; и &quot;Подвариантите&quot; да се попълват към вече добавен &quot;Продукт&quot;!<br />
&nbsp;&nbsp; <span style="color: #cc0000">Внимание :</span>&nbsp;Когато добавите продукт, вие автоматично ставате негов <span style="font-weight: bold">редактор</span>. Това ще ви позволи да го модифицирате - да променяте неговите характеристики, изображения, варианти и др. от страницата му.&nbsp;До 30 минути след добавяне на продукта може да се променя името/компанията/категорията му.<br />
<br />
&nbsp;&nbsp; Повече информация за добавяне/променяне на продукт може да <a shape="rect" href="Guide.aspx#infaeproducts" target="_blank">прочетете тук</a>.<br />
&nbsp;&nbsp; Правилата за добавяне/променяне на продукт са <a shape="rect" href="Rules.aspx#rulesaeprod" target="_blank">описани тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'23')
,(N'aboutReports', N'<p style="text-align: center; margin: 0px">This page is for viewing/answering/resolving user reports about the site.</p><br />
<span style="color: #009900">When viewing report use the following buttons for :</span><br />
Make Viewed : Click it to indicate that the report has been seen by administrator and he is working on it.<br />
Resolve : Use it to indicate that the report has been resolved, that way no more replies can be exchanged between the user and the administrators.<br />
Reply : Use it to write reply to user about the report.<br />
Delete comment : If it`s a spam report clicking this button will delete the comment and indicate it as resolved, it`ll also send warning to the user who wrote the comment.<br />
Delete suggestion : Analogy to delete comment.<br />
<br />
&nbsp;&nbsp; If the report is about edit suggestion, clicking on ‘Edit suggestion’ link, will show the suggestion.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'24')
,(N'aboutSiteTexts', N'<p style="text-align: center; margin: 0px"><span style="color: #009900">From here site texts can be modified.</span></p><br />
<span style="color: #009900">Different type`s when adding text mean :</span><br />
1. Text : Use it to add texts with names that are shown in the table for missing texts. If there are no missing texts then don’t use this type, because the text wont show in the site`s pages.<br />
2. News : Use it to add news. News are shown in home page.<br />
3. Question and answer : Add`s new question and answer to FAQ page.<br />
4. Rule : Add`s new rule to rules page.<br />
5. Guide text: Information on how to work with site. They are shown in Guide page.<br />
6. Warning pattern : Text pattern used when sending warnings to users. They are shown in Warnings page.<br />
7. Report pattern : Text pattern used when answering user reports. They are shown in reports page.<br />
<br />
Link ID : The id with which the text will be linked from other pages is the site. Currently used for rules, faq and guide texts.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'25')
,(N'aboutStatistics', N'<p style="text-align: center; margin: 0px">From here can be seen statistics aswell as online users at the moment</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'26')
,(N'aboutUserReporting', N'Моля, докладвайте проблеми само в настоящата страница.<br />
 <br />
 Може да сигнализирате за технически проблеми, неподходящо съдържание или неспазване на правилата. Опишете проблема в детайли. <br />
 <br />
 Повече информация можете да <a href="Guide.aspx#infabreports" target="_blank">прочетете тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'27')
,(N'aboutGeneralReporting', N'Тук може да докладвате за проблеми в сайта, неподходящо съдържание или неспазване на правилата.<br />
 <br />
 Моля, посочете на коя страница е проблема и го опишете в детайли, за да може да го открием бързо и реагираме адекватно. <br />
 <br />
 Повече информация ще <a href="Guide.aspx#infabreports" target="_blank">намерите тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'28')
,(N'Информация относно добавяне/редактиране на компания', N'&nbsp;&nbsp;&nbsp;Елементът&nbsp;„компании”&nbsp;позволява&nbsp;по-лесна организация на продуктите в сайта.&nbsp;<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-size: 13pt; color: #009933; ">Информация относно добавяне на компания</span></p><br />
&nbsp;&nbsp; Само потребители, които са регистрирани с посочена ел.поща могат да добавят компании!<br />
&nbsp;&nbsp; Компании могат да се добавят при кликване на връзката&nbsp;„Добави компания” в „Меню”.<br />
 <br />
&nbsp;&nbsp; Когато потребител добави компания, той автоматично става неин <span style="font-weight: bold; ">редактор</span>, което му дава правото да я модифицира. В страницата за редакторски права на потребител, могат да се видят продукти/компании, на които е <span style="font-weight: bold; ">редактор</span>.<br />
 <br />
 <span style="font-style: italic; ">Бележка : Потребителят, който е добавил компания, няма да може да редактира продуктите, които са добавени към нея, освен ако не ги е добавил той.</span><br />
<br />
&nbsp;&nbsp; &nbsp;<span style="font-size: 13pt; color: #009933; ">Полета :</span><br />
<br />
1.&nbsp;„Име”&nbsp;(Официалното име на компанията) <br />
 2.&nbsp;„Сайт”&nbsp;- Официалния сайт на компанията, който ще препрати потребителите, ако искат да разберат повече за нея.<br />
 3.&nbsp;„Описание”&nbsp;- Кратко описание на компанията, което трябва да информира потребителите с какво се занимава тя. Ако описанието е дълго, може по-голямата му част да бъде добавена като характеристика.<br />
4. &quot;Друго име&quot; - Други имена, под които е известна компанията.<br />
Пример : продукта &quot;Sony Playstation 3&quot; е известен и като &quot;PS 3&quot;; компанията &quot;LG&quot; е известна и като &quot;Lucky-Goldstar&quot; и &quot;Lucky&quot;. Компаниите могат да бъдат открити и по другите им имена.<br />
<br />
&nbsp;&nbsp; Характеристики, категории и изображения могат да се добавят едва след добавяне на продукта.<br />
 <br />
 <span style="font-style: italic; ">Бележка :&nbsp;Името на компанията може да бъде променено до 30 минути след като е добавена компанията! След това само администраторите ще могат да го променят.</span><br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-size: 13pt; color: #009933; ">Информация относно редактиране на компания</span></p><br />
&nbsp;&nbsp; Компаниите могат да бъдат редактирани от техните страници. Ако потребителят е <span style="font-weight: bold; ">редактор</span>, ще са показани две възможности – „Добави” и „Промени”, точно след описанието на компанията.<br />
 <br />
 1. „Характеристики” - Като характеристика може да се добави всякаква допълнителна информация за компанията. <br />
 <span style="text-decoration: underline; ">2. „Категории” - За да могат да се добавят продукти към компанията, е нужно да се посочи в кои категории тя може да има такива. Това е необходимо и важно, за да се покаже компанията в падащото меню, когато се добавя продукт!</span><br />
 <br />
 <span style="font-style: italic; ">Бележка : Ако не са добавени категории, в които компанията може да има продукти, потребителите няма да могат да добавят такива към нея.</span><br />
 <br />
 3. „Изображения” - Към всяка компания може да се добавят ограничен брой изображения. Едно от тях може да е маркирано като лого. То винаги ще се показва отпред в страницата на компанията, както и на други места. Изображенията могат да бъдат изтривани.<br />
<br />
&nbsp;&nbsp; Правила за добавяне/редактиране на компании могат да бъдат <a href="Rules.aspx#rulesaemaker" target="_blank">прочетени тук</a>.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infaaemaker')
,(N'Информация свързана с регистрацията', N'&nbsp;&nbsp; Потребители, които искат да имат допълнителни възможности, ще трябва да се регистрират!&nbsp;<br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">Права, които потребителите получават при регистрация :</span></font></font></p><br />
1.&nbsp;Участие във форума на всеки продукт в сайта (започване на теми, писане на коментари).<br />
2. Изпращане на лични съобщения към потребителите.<br />
3. Добавяне на продукти и компании.<br />
4. Оценяване на продукти и мнения.<br />
5. Даване на предложения за сайта.<br />
6. Изпращане на доклади относно проблеми със сайта, или потребители, които не спазват правилата.<br />
7. Изпращане на „предложения за промяна” към <span style="font-weight: bold; ">редактори</span>.<br />
 <br />
&nbsp;&nbsp; Има два типа регистрация, чиято разлика е в правата, които се получават :
<ul style="padding-left:30px;">
<li>„Писател” (без посочена ел.поща) - Получава всички права, без 3,5,7;</li>
<li>„Потребител”&nbsp;(с посочена ел.поща) - Всички права.</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"><span style="color: #009933; font-size: 13pt; ">Полета при регистр</span></font></font><font color="#009900"><font color="#404040"><span style="color: #009933; font-size: 13pt; ">ация</span></font></font></p><br />
1. „Потребителско име” - Името, с което потребителя ще е известен в сайта.<br />
 <br />
 <span style="text-decoration: underline; font-style: italic; ">Важно : Забранено е използването на обидни и нецензурни изрази за име!</span><br />
 <br />
 2. „Парола” (парола на профила) - Колкото по-трудна, толкова по-добре.<br />
 3. „Повтори паролата” - За да е сигурно, че желаната парола е въведена.<br />
 4. „Поща” (електронният пощенски адрес на потребителя) - &nbsp;При въвеждане потребителят ще има всички права, иначе важат правилата, описани по-горе.<br />
 <br />
 <span style="font-style: italic; ">Бележка : Ако потребител не въведе актуалния си пощенски адрес, не може да активира профила, защото няма да получи връзка за активация. Електронната поща ще се използва при „забравена парола”</span>.<br />
 &nbsp;<br />
 5. „Таен въпрос”- Въпрос, на който потребителя ще трябва да отговори, в случай, че забрави паролата, за да получи нова. <br />
 6. „Отговор” (отговора на тайният въпрос)<br />
 7. „Captcha”- В това поле ще трябва да се въведат буквите, които са на изображението. Няма значение дали са големи или малки. Това е предпазване от зловредни програми!<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">След регистрация</span></font></font></p><br />
&nbsp;&nbsp; При успешно регистриране, системата ще впише потребителя автоматично. Новите възможности ще се появят и в лентата с менюта, и в самото „меню”.<br />
&nbsp;&nbsp; Лична информация като парола, поща и др. може да бъде променяна от страницата на потребителя.<br />
 <br />
 <span style="font-style: italic; ">Важно : Всички права, които потребителят получава с регистрацията, както и профилът му, могат да бъдат премахнати от администраторите, ако правилата не се спазват!</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infar')
,(N'Информация свързана с личните съобщения', N'&nbsp;&nbsp; Тъй като основната идея на сайта са потребителските мнения, даваме възможност на потребителите да обменят информация и под формата на „лични съобщения”, но трябва да са регистрирани и вписани, за да могат да ползват услугата!<br />
 <br />
&nbsp;&nbsp; Съобщения могат да се изпращат от страницата за „лични съобщения” на потребителя, както и от падащото меню „Действия” от мненията в страницата на продукт.<br />
 <br />
 <span style="font-style: italic; ">Бележка : „Лични съобщения” не могат да се пращат към: администратори, гости, потребители, блокирали автора и потребители, които са си блокирали кутията за съобщения!</span><br />
 <br />
&nbsp;&nbsp; Получените и изпратените съобщения могат да се видят от страницата за „лични съобщения” на потребителя. Когато той получи ново съобщение, ще се появи иконка до бутона за „Изход” в лентата с менюта.<br />
 <br />
&nbsp;&nbsp; Бутон „Блокирай” - Блокира съобщенията от посочения потребител;<br />
 <br />
&nbsp;&nbsp; „Блокирай” кутията за съобщения - Когато потребител блокира кутията за съобщения, няма да може да получава съобщения от други потребители.<br />
 <br />
 <span style="font-style: italic; ">Бележка : Това не важи за администраторите, техните съобщения се получават винаги!</span><br />
 <br />
 <span style="font-style: italic; ">Бележка : Администраторите не могат да бъдат блокирани!</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infapm')
,(N'aboutCompanyTypes', N'<p style="text-align: center; margin: 0px">Maker types are modified from here.</p>
<p style="text-align: left; margin: 0px"><br/></p>Description must be short as possible, 3-4 rows at maximum.<br />
<br />
<span style="font-style: italic">NOTE : If you delete type with which there are makers then they`ll become of type ''other''.</span>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'33')
,(N'aboutLogIn', N'<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #003399; ">Впишете се, за да можете да :</span></p><br />
&nbsp;&nbsp; • Оценявате продукти/мнения и изпращате лични съобщения <br />
&nbsp;&nbsp; • Добавяте продукти/компании<span style="color: #ff0000; ">* </span>и&nbsp;давате мнения<br />
&nbsp;&nbsp; • Пишете предложения и докладвате за нередности<br />
&nbsp;&nbsp; • Участвате в форумите на продуктите<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 320px; ">&nbsp;… и още</p> <font color="#FF0000"><br />
</font>&nbsp;&nbsp; Ако сте забравили паролата си, <a href="ForgottenPassword.aspx" target="_blank">натиснете тук</a>.<br />
&nbsp;&nbsp; Ако искате да се регистрирате, посетете <a href="Registration.aspx" target="_blank">тази страница</a>.<br />
<br />
<span style="color: #ff0000; ">&nbsp;&nbsp; *</span>&nbsp;Само ако сте регистрирани с посочена ел.поща.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'34')
,(N'aboutGuide', N'&nbsp;&nbsp; Тук са описани различните елементи на сайта и как се работи с тях. Моля, прочетете ги, както и <a href="Rules.aspx" target="_blank">правилата</a>. Ако не можете да откриете отговор на въпрос, посетете и&nbsp;<a href="FAQ.aspx" target="_blank">тази страница</a>. Ако все още не намирате отговор - не се притеснявайте да ни попитате от страницата „<a href="AboutUs.aspx" target="_blank">За сайта</a>”.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'42')
,(N'Информация относно добавяне/редактиране на продукт', N'&nbsp;&nbsp;&nbsp;Преди да дадете мнение за продуктите, е необходимо да ги добавите.<br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Информация относно добавяне на продукт</span></p><br />
&nbsp;&nbsp; Продукти могат да се добавят от потребители, които са регистрирани с посочена ел.поща!<br />
<br />
&nbsp;&nbsp; Когато потребител добави продукт, той автоматично става негов <span style="font-weight: bold">редактор</span>, което му позволява да го модифицира. В страницата за редакторски права на потребител могат да се видят кои продукти/компании може да редактира.<br />
<br />
&nbsp;&nbsp; Има четири места, от които може да се добави продукт :<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1.&nbsp;От „Меню” при кликване на опцията „Добави продукт”.&nbsp;<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2. От страницата на „категория” при кликване върху „Добави продукт”.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3. От страницата на „компания” при кликване върху „Добави продукт”. <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;4. От страницата на „продукт” при кликване върху „Добави продукт”.<br />
<br />
&nbsp;&nbsp; <span style="font-size: 13pt; color: #009933; ">&nbsp;Полета:</span><br />
<br />
1.&nbsp;„Име”&nbsp;(името на продукта) - Ако в името на продукта има цифри и/или единични символи, може да се добави името на „компанията” отпред (Пример : Nokia N73C).<br />
2.&nbsp;„Компания”&nbsp;(компанията на продукта) - Ако продукта се добавя към „компания”, това поле ще е недостъпно, тъй като „компанията” е предварително зададена.<br />
<br />
<span style="font-style: italic">Бележка : Ако в падащото меню липсва „компания”, към която искате да добавите продукта, значи, че „компанията” не може да има продукти в тази категория. Редакторите на „компанията” определят в кои категории тя може да има продукти. За повече информация <a shape="rect" href="Guide.aspx#infts" target="_blank">кликнете тук</a>.</span><br />
<br />
3.&nbsp;„Категория”&nbsp;(категорията, в която ще е продукта) - Ако продукт се добавя към „категория”, това поле ще е недостъпно и категорията ще е предварително зададена. Ако продукт се добавя към „компания”, в полето ще са добавени категории, в които „компанията” може да има продукти. <br />
<br />
<span style="font-style: italic">Бележка : Ако в падащото меню липсва „категория”, съответстваща на продукта, може да :<br />
</span>
<ul style="padding-left:30px;">
<li><span style="font-style: italic; ">изпратите предложение към редактора на „компанията” (да добави съответната категория);</span></li>
<li><span style="font-style: italic; ">изберете „Други -&gt; Неопределени” и от новопоявилото се меню да изберете желаната „категория” за продукта. В този случай редакторът на „компанията” ще получи системно съобщение за вашето желание (продукта да се намира в желаната „категория”) и ако я добави, системата автоматично ще премести продукта в нея.</span></li></ul><span style="font-style: italic">Предупреждение : Моля, всички потребители да процедират коректно, за да няма санкции!</span><br />
<br />
4.&nbsp;„Сайт” -&nbsp;Официалният сайт на продукта. Ако не го знаете оставете полето празно.&nbsp;Може да се променя в страницата на продукта.<br />
<font color="#333333"><br />
</font>5.&nbsp;„Описание”&nbsp;(кратко описание на продукта) - Ако техническите характеристики на продукта не са дълги, могат и те да бъдат добавени към описанието. В противен случай е по-добре да бъдат добавени като &quot;характеристика&quot;. Максималната дължина е ограничена, за да се добавя само най-важната информация! Останалото може да се добави като характеристика.<br />
<br />
&nbsp;&nbsp; Изображения, характеристики, варианти, подварианти и други имена могат да се добавят, едва след добавяне на продукта.<br />
<br />
<span style="font-style: italic">Бележка :&nbsp;Името, категорията и компанията могат да бъдат променяни до 30 минути след като е добавен продукта! След това само администраторите ще могат да ги променят.</span><br />
<br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Информация за редактиране на продукт</span></p><br />
&nbsp;&nbsp; Продуктите могат да бъдат редактирани от техните страници. Ако потребител има права за това, ще са показани две възможности – „Добави” и „Промени”, точно след описанието на продукта.<br />
<br />
1. „Характеристики” - Като характеристика може да се сложи всякаква допълнителна информация относно продукта, напр.: технически параметри, проблеми и други. Трябва да се знае, че за характеристиките може да се дават мнения, които могат да бъдат сортирани.<br />
<br />
<span style="font-style: italic">Бележка : Ако за характеристика са написани мнения, редакторите няма да могат да променят нейното име, но ще имат опцията за изтриването и.</span><br />
<br />
2. „Изображения” - Към всеки продукт може да се добавят ограничен брой изображения. Едно от тях може да е главно. То винаги ще се показва отпред в страницата на продукта, както и на други места. Изображенията могат да бъдат изтривани.<br />
3. „Варианти” - За да не се дублират продукти с една компания и име, но с малка разлика в техническите параметри, <span style="font-weight: bold">редакторите</span> имат възможност да добавят „варианти” към продукта. Във всеки вариант могат да се опишат разликите спрямо главния продукт.<br />
<br />
<span style="font-style: italic">Пример : Главен продукт <div style="border-bottom-style: none; border-right-style: none; display: inline; border-top-style: none; border-left-style: none">„автомобил &nbsp;‘<span lang="EN-US">Audi A4 B5</span>’ ” с варианти :<span lang="EN-US">дизел</span>; бензин.<br />
</div></span><br />
4. „Подварианти” - Вариантите могат да имат „подварианти” с по-специфични разлики.<br />
<br />
<span style="font-style: italic">Пример :&nbsp;<div style="border-bottom-style: none; border-right-style: none; display: inline; border-top-style: none; border-left-style: none">Вариант ‘Аudi A4 B5 – дизел’ с подварианти на двигателя : 1.9 TDI; 2.5 TDI.<br />
</div></span><br />
&nbsp;&nbsp; Потребителите имат право да дават мнения и за вариантите, и за подвариантите. Мненията могат да бъдат сортирани и по двете. Описанието им може да бъде променяно, след като са добавени.<br />
<br />
&nbsp;&nbsp; Правилата за добавяне/редактиране на продукти, могат да бъдат <a shape="rect" href="Rules.aspx#rulesaeprod" target="_blank">прочетени тук</a>.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infaeproducts')
,(N'Информация свързана с предложенията за промяна', N'&nbsp;&nbsp; Чрез „предложенията” за промяна потребителите могат да се свържат с <span style="font-weight: bold; ">редакторите</span> и да им предложат да обновят информацията за продукт/компания. Тъй като много от продуктите се актуализират периодично (излизат нови варианти), информацията в сайта трябва да се допълва, и това е средството, чрез което потребителите помагат на <span style="font-weight: bold; ">редакторите</span>.<br />
 <br />
&nbsp;&nbsp; Такива предложения могат да пишат потребители, които са регистрирани с ел.поща!<br />
<ul style="padding-left:30px;">
<li>Предложение за&nbsp;„продукт”&nbsp;&nbsp;може да се изпрати от страницата на продукта. Опцията се намира над панела за писане на мнения;</li>
<li>Предложение за&nbsp;„компания”&nbsp;може да се изпрати от нейната страницата. Опцията се намира под категориите;</li>
<li>Потребителят може да управлява предложенията за промяна от страницата за тях;</li>
<li>Когато предложение е пратено към&nbsp;<span style="font-weight: bold; ">редактор</span>, той има ограничено време, в което да го приеме или откаже, иначе системата ще го откаже автоматично!</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">Възможности на </span></font></font><font color="#009900"><font color="#404040"><span style="font-weight: bold; font-size: 13pt; color: #009933; ">редакт</span></font></font><font color="#009900"><font color="#404040"><span style="font-weight: bold; font-size: 13pt; color: #009933; ">ора</span></font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; "> при получаване на предложение</span></font></font></p> <br />
„Отговори” - Може да отговори на потребителя, който е пратил предложението, като му поиска повече информация;<br />
 „Приеми” – Задължава <span style="font-weight: bold; ">редактора,</span>&nbsp; че ще обнови информацията;<br />
 <br />
 <span style="font-style: italic; ">Бележка : Ако след определеното време редактора не обнови информацията, потребителят,&nbsp; пратил предложението, може да изпрати доклад към администраторите!</span><br />
 <br />
 „Откажи” - Отбелязва, че <span style="font-weight: bold; ">редакторът</span> не приема предложението и няма да обнови информацията според него;<br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 40px; "></p> <span style="font-style: italic; ">Бележка : Редакторът трябва да приеме или откаже предложението в определен срок (примерно 3 дни). След неговото изтичане предложението ще бъде отказано автоматично от системата и авторът ще бъде уведомен за това със системно съобщение.</span><br />
 <br />
 <span style="font-style: italic; ">Бележка : В случай, че предложение е отказано, независимо от причината, и потребителят е предложил актуално обновяване на варианти или категории, може да докладва отказа на администраторите. Но ако предложението не представлява полезно допълнение към съществуващата информация, администраторите няма да вземат отношение към докладите на потребителя!</span><br />
<br />
„Изтрий” - &nbsp;Изтрива предложението, но ако преди това не е прието/отказано, го маркира като „Отказано” &nbsp;за потребителя, който го е изпратил.<br />
 <br />
&nbsp;&nbsp; Когато се получи „предложение” за промяна или „отговор” на предложение, ще се появи иконка до бутона за „Изход” в лентата с менюта.&nbsp; По този начин потребителите ще бъдат уведомени, че има нова информация за предложенията. <br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "> <span style="color: #009933; font-size: 13pt; ">Бележки</span></p> <br />
 1. Ако „предложение” не е изпратено в съответната форма, но е изпратен доклад за него, то няма да се вземе под внимание от администраторите!<br />
 2. Не пишете „предложение” като „мнение” за продукт - ако го направите, най-вероятно ще бъде маркирано като „нарушение” и изтрито!<br />
 3. <span style="font-weight: bold; ">Редакторите</span> също могат да докладват на администраторите за изпратени към тях неактуални предложения!<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infts')
,(N'Информация за системните съобщения', N'&nbsp;&nbsp; Понякога администраторите или потребителите извършват действие, което е свързано с друг потребител. За да го информира, системата автоматично му праща съобщения. <br />
 <br />
 <span style="font-style: italic; ">Пример : Ако администратор премахне право на потребител, системата ще му прати съобщение за това.</span><br />
 <br />
&nbsp;&nbsp; Системните съобщения могат да бъдат видяни както в страницата на потребителя (профила), така и чрез иконка до бутона за „Изход” в лентата с менюта.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infsm')
,(N'Информация относно предупрежденията', N'&nbsp;&nbsp; Предупрежденията са публичният начин за санкциониране на потребителите, когато нарушават правилата! <br />
&nbsp;&nbsp; Предупрежденията се показват в страницата на потребителя (профила) и се виждат от всички потребители!<br />
 <br />
 <span style="font-style: italic; ">ВАЖНО : След получаване на определен брой (между 2 и 5) предупреждения за едно и също неправомерно действие, системата автоматично ще премахне съответното право на потребителя! Ако общият брой на предупрежденията достигне определено число, профилът на потребителя ще бъде изтрит!</span><br />
 <br />
 <span style="font-style: italic; ">Пример 1 : Ако потребител е получил 2 предупреждения за нарушаване правилата за писане на мнения, при 3-то предупреждение системата автоматично му отнема това право.<br />
 <br />
 Пример 2 : Ако потребител получи 10 предупреждения за различни нарушения, профилът му ще бъде изтрит от системата.</span><br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infwarns')
,(N'Информация за известията', N'&nbsp;&nbsp; С „Известията” потребителите биват уведомявани за нова информация относно продукт/компания/форум/тема. Така отпада необходимостта да се проверява периодично дали&nbsp;има нова информация, от която се интересувате.<br />
 <br />
&nbsp;&nbsp; Ако искате да бъдете известени за нова информация относно продукт/компания/форум/тема, посетете съответната страница и активирайте бутона „Извести”.<br />
<br />
&nbsp;&nbsp; Ще получавате информация, когато :
<ul style="margin-top:0px;">
<li>(за Продукт) &nbsp;: има нови мнения за него;</li>
<li>(за Компания) : има добавени нови продукти;</li>
<li>(за Форум) &nbsp; &nbsp;: има започнати нови теми;</li>
<li>(за Тема) &nbsp; &nbsp; : има написани нови коментари;</li></ul>&nbsp;&nbsp; Когато има нова информация, потребителите ще бъдат известени, като се появи иконка до бутона за „Изход” в лентата с менюта. В страницата с „известия” потребителите могат да видят за точно кой продукт/компания/форум/тема&nbsp;има нова информация.<br />', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infnotifs')
,(N'Обща информация за редакторските права и прехвърлянето им', N'&nbsp;&nbsp; Потребител става <span style="font-weight: bold; ">редактор</span>, когато добави продукт или компания. Това му дава право да го модифицира, но и поема отговорността да го обновява, когато е необходимо, докато е негов <span style="font-weight: bold; ">редактор</span>! <br />
 <br />
<span style="font-weight: bold; ">&nbsp;&nbsp; Редакторите</span> могат да се откажат от правото си на такива по тяхно желание. Това става като влязат в страницата за редакторските права (от менюто на потребителския профил).<br />
 <br />
<span style="font-weight: bold; ">&nbsp;&nbsp; Редакторските</span> права на „потребител” се виждат от всички, които посетят неговата страница(на профила) и активират връзката „Редакторски права”. Тя не съществува, ако той не е регистриран с ел.поща!<br />
 <br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "> <span style="font-size: 13pt; color: #009933; ">Прехвърляне на права</span></p> <br />
&nbsp;&nbsp; От страницата за „Редакторски права” те могат да се прехвърлят. Това дава възможност всяко право да се предаде на друг потребител, в случай, че актуалния <span style="font-weight: bold; ">редактор</span> се отказва от него.<br />
 <br />
&nbsp;&nbsp; <span style="color: #009900; ">Бележки :</span><br />
 1. Когато дадено право се прехвърля към друг потребител, той има ограничено време да го приеме, след което системата автоматично прекъсва прехвърлянето.<br />
 2. <span style="font-weight: bold; ">Редактор</span> може да модифицира продукт/компания, докато се прехвърля правото за него.<br />
 3. Когато нов потребител приеме право, той става <span style="font-weight: bold; ">редактор</span>, а предишния го губи.<br />
 4. Настоящият <span style="font-weight: bold; ">редактор</span> може да прекъсне „прехвърляне”, докато нов потребител не го е приел или отказал.<br />
 5. <span style="font-weight: bold; ">Редактор</span> не може да прехвърля права към потребител, когото е блокирал.<br />
 <br />
 <span style="font-style: italic; ">Важно : Тъй като редактора е отговорен за обновяване информацията за компании /продукти, върху които има права, съществува време, през което ако не се впише поне веднъж в сайта, другите потребители ще могат да му отнемат редакторските права!<br />
 <br />
 Пример : Ако редактор не се впише нито веднъж за период от 3 месеца, други потребители ще могат да вземат всички негови редакторски права.<br />
 <br />
</span>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; color:#c02e29; "><u>Редакторски права могат да бъдат давани или отнемани от администраторите по всяко време!</u></span></p>', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'ginfrt')
,(N'Обща информация', N'<p style="text-align: center; margin: 0px"><span style="font-weight: bold; color:#c02e29; font-size:12pt;"><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></span></p><br />
<p style="text-align: center; margin: 0px"><span style="font-size: 13pt; color: #009933; ">Потребителите, които не спазват правилата на сайта, ще бъдат санкционирани от администраторите по следните начини:</span></p><br />
1. Изпращане на предупреждение към потребител, което ще се вижда от всички останали.<br />
2. Премахване на право – напр., ако потребител наруши правилата за писане на мнения, администраторите могат да премахнат правото му за това.<br />
3. Изтриване на потребителски профил - ако потребител системно нарушава правилата, профилът му може да бъде изтрит без предупреждение.<br />
4. Блокиране на IP адрес (IP ban) - Ако потребители нарушават правилата от един IP адрес, той може да бъде блокиран. Потребителите няма да могат да пишат мнения, да се регистрират и да се вписват от такъв адрес.<br />
<br />
<span style="font-style: italic">Важно : Няма точен ред, който ще се следва при санкциите и затова профили и права могат да бъдат премахвани без предупреждения! Ако потребител смята, че е санкциониран неправилно, трябва да се свърже с администраторите на сайта чрез доклад или формата в страницата „<a shape="rect" href="AboutUs.aspx" target="_blank">За сайта</a>”.</span><br />
<br />
&nbsp;&nbsp; Това се прави с цел да се предотвратят щети, нанесени от некоректни потребители! Длъжни сме да направим всичко възможно, за да се чувствате комфортно в този сайт!<br />
<br />
<p style="text-align: center; margin: 0px"><span style="color: #009933; font-size: 13pt; ">Ограничения, за които потребители трябва да знаят!</span></p><br />
1. „Гостите” и регистрирани потребители, ненабрали необходим брой мнения, ще трябва да изчакват определено време между писането им.<br />
2. Броят регистрации от една ел.поща и от един IP адрес е ограничен.<br />
3. Броят на мненията, които всеки потребител или гост може да има за всеки продукт, е ограничен. От един IP адрес могат да бъдат написани определен брой мнения.<br />
4. „Гостите” не могат да отговарят на мнения.<br />
5. Предложенията към сайта, които потребителят може да праща, са ограничени.<br />
6. Между две последователни добавяния на продукти/компании/теми трябва да мине известно време.<br />
7. Регистрираните потребители имат право да оценяват определен брой мнения за всеки продукт/тема.<br />
8. Регистрираните потребители имат право да оценяват определен брой мнения на ден.<br />', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulgi')
,(N'aboutReportUser', N'Моля, докладвайте само нарушения на този потребител.<br />
 <br />
 Опишете нарушението на правилата, за да реагираме адекватно.<br />
 <br />
 Повече информация относно писането на доклади може да <a href="Guide.aspx#infabreports" target="_blank">видите тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'abru')
,(N'aboutWarnings', N'&nbsp;&nbsp;&nbsp;When user break rule send him warning before anything else. If role need to be removed, send warning to user and after that, remove his role. Describe why the user is receiving it, because everyone can see his warnings.&nbsp;<br />
&nbsp;&nbsp; Use ''general'' warning, only when it`s not connected with any of the roles.', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutwarnings')
,(N'aboutEditSuggestions', N'<p style="text-align: center; margin: 0px"><span style="font-family: times new roman,times,serif; color: #003399; font-size: 16pt">Тук можете да управлявате предложенията за промяна, които сте получили или изпратили.</span></p><br />
<p style="text-align: right; margin: 0px">Повече информация може да <a shape="rect" href="Guide.aspx#infts" target="_blank">прочетете тук</a>.</p>', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutes')
,(N'aboutReportEditSuggestion', N'Моля, докладвайте за проблеми свързани с избраното предложение.<br />
 <br />
 Докладът може да е за :<br />
 1. Отказ на <span style="font-weight: bold; ">редактора</span> да обнови важна информация относно продукт/компания.<br />
 2. Потребител „спами” или пише предложения, които нямат връзка с дадения продукт/компания.<br />
 3. <span style="font-weight: bold; ">Редактор</span> или потребител използва обидни и нецензурни изрази.<br />
 Повече информация може да <a href="Guide.aspx#infabreports" target="_blank">прочетете тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'aboutres')
,(N'Правила за добавяне/редактиране на продукт', N'<ul style="padding-left:30px;">
<li>Забранено е попълването на грешна, подвеждаща и некоректна информация за продуктите!;</li>
<li>Строго забранено е използването на обидни и нецензурни думи!;</li>
<li>Забранени са каквито и да е реклами!;</li>
<li>Препоръчително е, ако продукта има варианти и подварианти, те да бъдат добавени, за да могат потребителите да дават мнения и за тях!</li>
<li>Забранено е използването на защитена с авторско право информация!<br />
</li></ul> &nbsp;&nbsp; Неспазването на правилата ще доведе до санкции!<br />
<p class="MsoNormal" style="margin-bottom:0cm;margin-bottom:.0001pt;text-indent:4.5pt;line-height:normal"><b style="mso-bidi-font-weight:normal">&nbsp;&nbsp; </b><b style="mso-bidi-font-weight:normal"><u>Редакторски права могат да бъдат давани или отнемани от администраторите по всяко време!<o:p /></u></b></p>  <br />
 &nbsp;&nbsp; Повече информация за добавяне/редактиране на продукти може да <a href="Guide.aspx#infaeproducts" target="_blank">прочетете тук</a>.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29;"><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesaeprod')
,(N'Правила за добавяне/редактиране на компания', N'<ul style="padding-left:30px;">
<li>Забранено е попълването на грешна, подвеждаща и некоректна информация за компанията!;</li>
<li>Строго забранено е използването на обидни и нецензурни думи!;</li>
<li>Забранени са каквито и да е реклами!;</li>
<li>Желателно е всички категории, в които компаниите имат продукти, да бъдат добавени, за да може да се добавят продукти в тях;</li>
<li>Забранено е използването на защитена с авторско право информация!<br />
</li></ul> &nbsp;&nbsp; Неспазването на правилата ще доведе до санкции!<br />
  <br />
<span style="font-weight: bold; ">&nbsp;&nbsp; &nbsp;<u>Редакторски права могат да бъдат давани или отнемани от администраторите по всяко време!</u></span><br />
  <br />
 &nbsp;&nbsp; Повече информация за добавяне/редактиране на компании може да <a href="Guide.aspx#infaaemaker" target="_blank">прочетете тук</a>.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="font-weight: bold; font-size: 12pt; color: #c02e29; "><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesaemaker')
,(N'Обща информация за докладите', N'&nbsp;&nbsp;&nbsp;„Докладите”&nbsp;са инструмента, с който можете да информирате администраторите за проблеми. За проблем считаме: „бъг” (техническа грешка), грешна информация, проблем със страница или нещо, което нарушава правилата.<br />
 <br />
&nbsp;&nbsp; Потребител трябва да е регистриран и вписан, за да може да пише доклади! <br />
 <br />
&nbsp;&nbsp; Доклади за нередност могат да бъдат писани от няколко места:<br />
<ul style="padding-left:30px;">
<li>Страница на „категория” - Потребителите трябва да докладват за проблеми, свързани само със страницата на избраната категория;</li>
<li>Страница на „компания” - Потребителите трябва да докладват за проблеми, свързани само със страницата на избраната компания;</li>
<li>Страница на „продукт” - Потребителите трябва да докладват за проблеми, свързани само със страницата на избрания продукт;</li>
<li>Страницата за „доклади на потребител” - От тази страница потребителите могат да докладват за проблеми, които не са свързани с някоя от по-горните страници.</li>
<li>Страницата на „потребител” - Когато потребител посещава страницата на друг потребител, той има възможност да докладва за нарушенията му;</li>
<li>Страница на „тема” - Потребителите трябва да докладват за проблеми, свързани само с темата.<br />
</li></ul> <span style="font-style: italic; ">Пример : Доклад, написан от страницата на компанията&nbsp;„Audi”, е относно страницата на „Audi”.</span>&nbsp;&nbsp;<br />
 <br />
&nbsp;&nbsp;&nbsp;Има два типа доклади - за нередност, и „нарушение”. Докладите за „нарушение” засягат потребителските мнения, коментари или предложения, и ако администраторите преценят, ги изтриват.<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">Препоръки при писане на доклад</span></font></font></p>
<ul style="padding-left:30px;">
<li><font color="#009900"><font color="#404040">Ако потребителят докладва за проблем от своята страница за доклади, то трябва изрично да се спомене в коя страница е срещнал&nbsp;</font></font>проблема<font color="#009900"><font color="#404040">;</font></font></li>
<li><font color="#009900"><font color="#404040">Ако има функционален проблем, опишете възможно най-точно вашите действия преди той да възникне;</font></font></li>
<li><font color="#009900"><font color="#404040">Опишете проблема в детайли;</font></font></li>
<li><font color="#009900"><font color="#404040">Когато докладвате нарушение на потребител, моля уверете се, че е нарушил правило!</font></font></li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"> </font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">След като е написан доклад</span></font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">а</span></font></font></p><font color="#009900"><font color="#404040"> <br />
&nbsp;&nbsp; Когато доклада е изпратен, администраторите ще вземат необходимите мерки за отстраняване на проблема. Доклада ще присъства в „страницата за доклади” на потребителя, който го е написал и оттам той ще може да следи развитието му. Ще са налични и няколко възможности за взаимодействие:
<ul>
<li><font color="#009900"><font color="#404040">Бутон „Изтрий” - При натискане изтрива доклада;&nbsp;<br />
<br />
</font></font><font color="#009900"><font color="#404040"><span style="font-style: italic; ">Бележка : Ако доклад, който не е приключен, бъде изтрит, то автоматично ще бъде маркиран като „приключен” и администраторите ще спрат работа по него!<br />
<br />
</span></font></font></li>
<li><span style="font-style: normal; "><font color="#009900"><font color="#404040">Бутон „Отговори” - При нужда от повече информация, администраторите ще пратят съобщение към потребителя. То ще се покаже под доклада, както и бутон „Отговори”. Чрез него ще осъществите обратна връзка с администраторите;<br />
<br />
</font></font><font color="#009900"><font color="#404040"><span style="font-style: italic; ">Бележка : Ако доклада е приключен или не е изпратено съобщение от администраторите, бутона няма да се вижда.<br />
<br />
</span></font></font></span></li>
<li><span style="font-style: normal; "><font color="#009900"><font color="#404040"><span style="font-style: normal; ">Бутон „Приключи” - При активирането му доклада ще се маркира като „Приключен” и администраторите ще престанат да работят по него.&nbsp;</span></font></font></span></li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"> </font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">Друга информация</span></font></font></p>
<ul style="padding-left:30px;">
<li>Администраторите не се ограничават с време за приключване или отговор на доклади!;</li>
<li>Максималната бройка на „неприключени” доклади за нередности и&nbsp;„нарушение” за всеки потребител е ограничена, т.е., ако тази бройка бъде достигната, не може да се пращат повече, докато част от тях не се приключат;</li>
<li>Докладите за&nbsp;„нарушение”&nbsp;не се показват в страницата за доклади на потребителя.<br />
</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><font color="#009900"><font color="#404040"> </font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">Прав</span></font></font><font color="#009900"><font color="#404040"><span style="font-size: 13pt; color: #009933; ">ила</span></font></font></p>
<ul style="padding-left:30px;">
<li>Докладите трябва да съдържат само информация, която е свързана с някакъв проблем в сайта!;</li>
<li>Строго забранено е използването на обидни и нецензурни думи!</li></ul><font color="#009900"><font color="#404040">&nbsp;&nbsp; Неспазването на правилата ще да доведе до санкции!<br />
</font></font></font></font>', GETUTCDATE(), N'information', 1, 3, GETUTCDATE(), 3, N'infabreports')
,(N'Жребият е хвърлен - "wiadvice.com" стартира!', N'&nbsp;&nbsp; Уважаеми потребители, след успешно тестване стартираме &quot;wiadvice.com&quot;! <br />
 <br />
&nbsp;&nbsp; Отне ни малко повече време, но успяхме да направим визуални и функционални подобрения - Сигурни сме, че с тях посещенията ви в сайта ще са по-приятни и пълноценни.<br />
 <br />
&nbsp;&nbsp; Попълването на продукти и компании тепърва започва - но както знаете, нещата зависят главно от вас! Вече имате възможността да покажете кои от тях ви интересуват и да изразите мненията си. Попълвайте на воля!<br />
 <br />
&nbsp;&nbsp; В скоро време започваме добавянето на нови възможности във визуалната и функционалната част - някои от тях ще са благодарение на изпратени от вас <a href="SuggestionsForSite.aspx" target="_blank">идеи и предложения</a> за сайта.&nbsp; <br />
 <br />
&nbsp;&nbsp; Очакваме ви...<br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsSiteStarted')
,(N'Добавен е форум за всеки продукт!', N'&nbsp;&nbsp; Приятели, вече имате възможността не само да давате мнения за всеки продукт, но и да го обсъждате в неговия персонален форум. Може да се възползвате от това, като от страницата на всеки продукт кликнете върху връзката &quot;форум&quot;, намираща се до &quot;редактори&quot; в дясно.<br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsForumIntroduced')
,(N'Правила за добавяне на тема/коментар', N'<ul>
<li>Темите трябва задължително да са свързани със съответния продукт;</li>
<li>Коментарите задължително трябва да са относно темата!;</li>
<li>Забранена е агресия в тона на коментарите!;</li>
<li>Строго забранено е използването на обидни и нецензурни думи!;</li>
<li><span style="color: #333333; ">Задължително е коментарите да са на Български език(кирилица)!;&nbsp;</span><br />
</li>
<li>Забранени са каквито и да е реклами!;</li>
<li>Забранено е използването на защитена с авторско право информация!</li></ul>&nbsp;&nbsp; <span style="color: #339900; ">Бележки :</span><br />
1. Между две последователни добавяния на теми трябва да мине известно време.<br />
2. Новорегистрираните потребители трябва да изчакват определено време между всеки коментар - това е необходимо, за да се забавят възможни „спам” атаки. При наличието на определен брой коментари, това ограничение ще отпадне.<br />
3. Маркирайте като „нарушение” всеки коментар, който нарушава правилата.<br />
<br />
&nbsp; Неспазването на правилата ще доведе до санкции!<br />
<br />
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="color: #c02e29; font-weight: bold; font-size: 12pt; "><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesTopic')
,(N'aboutTopicReporting', N'Моля, докладвайте проблеми само в настоящата тема.<br />
<br />
Може да сигнализирате за технически проблеми, неподходящо съдържание или неспазване на правилата.&nbsp;<br />
Моля, не докладвайте за нередности в коментарите на темата - тях маркирайте като &quot;нарушение&quot;!&nbsp;<br />
<br />
Повече информация може да <a href="Guide.aspx#infabreports" target="_blank">прочетете тук</a>.<br />', GETUTCDATE(), N'text', 1, 3, GETUTCDATE(), 3, N'textAboutTopicReporting')
,(N'Възможност за добавяне на връзки (линкове) към други сайтове.', N'&nbsp;&nbsp; От днес имате възможност да добавяте връзки към други сайтове, носещи конкретна информация за всеки продукт, който ви&nbsp;интересува! Опцията се намира до &quot;Форум&quot; и &quot;Редактори&quot; в страницата на продукта.<br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'newsAddLinks')
,(N'Правила за добавяне на връзки (линкове) към продуктите', N'&nbsp;&nbsp; &nbsp;За улеснение има възможност да се добавят връзки (линкове) към други сайтове, съдържащи специфична информация за продукта&nbsp;(подробни характеристики/форуми/мнения/ревюта и т.н.).<br />
<ul>
<li>Забранено е връзката да сочи към сайт :<br />
&nbsp;&nbsp; &nbsp; &nbsp;-&nbsp;рекламиращ продукта;<br />
&nbsp;&nbsp; &nbsp; &nbsp;-&nbsp;дублиращ информацията, която може да се намери на вече добавена връзка;&nbsp;<br />
&nbsp;&nbsp; &nbsp; &nbsp;-&nbsp;на фирми или търговски вериги, продаващи продукта (магазини);<br />
&nbsp;&nbsp; &nbsp; &nbsp;-&nbsp;който не е свързан с продукта.</li></ul>&nbsp;&nbsp; &nbsp;<span style="color: #339900; ">Бележки :</span>
<ul>
<li>Връзките може да бъдат добавяни само от потребители, регистрирани с посочена ел.поща;</li>
<li>Връзките може да се променят от редакторите и администраторите;</li>
<li>Моля, маркирайте като &quot;нарушение&quot; всички връзки, неспазващи правилата;</li>
<li>Между добавяне на две връзки трябва да мине определено време;</li>
<li>Моля, не злоупотребявайте с опцията! Дадена е за улеснение на потребителите;</li>
<li>Връзките, неотговарящи на правилата, ще бъдат премахвани!</li></ul>
<p style="margin-top: 0px; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; text-align: center; "><span style="border-collapse: collapse; color: #c02e29; font-family: trebuchet ms, verdana, arial, times new roman, georgia, tahoma, segoe ui; font-size: 15px; -webkit-border-horizontal-spacing: 2px; -webkit-border-vertical-spacing: 2px; "><b><u>Екипът на сайта не носи отговорност за информация, добавена и споделена от потребителите!</u></b></span></p>', GETUTCDATE(), N'rule', 1, 3, GETUTCDATE(), 3, N'rulesProdLinks')
,(N'Леки подобрения.', N'&nbsp;&nbsp; Здравейте! Добавена е опция за промяна на името/категорията/компанията на продукт от редактора, до 30 минути след като е добавен. Същото важи и за името на новодобавена компания. Опцията е въведена в случай, че потребител направи грешка при добавяне на продукт/компания и иска да я поправи.<br />
<br />
&nbsp;&nbsp; &nbsp;Направени са промени по сайта, за да изглежда добре на Internet Explorer 9.<br />
<br />
&nbsp;&nbsp; &nbsp;Наскоро излязоха нови версии на най-популярните браузъри (Chrome, Firefox, Internet Explorer...), имат значителни подобрения и поддръжка на нови технологии, поради което ви препоръчваме да актуализирате своя браузър до последната версия ако не сте го направили. Освен всичко останало сайтовете ще изглеждат така както би трябвало.<br />
<br />
&nbsp;&nbsp; &nbsp;Връзки, от които може да изтеглите последните версии :<br />
&nbsp;&nbsp; &nbsp; &nbsp; - Mozilla Firefox : <a href="http://www.mozilla.com/en-US/firefox/new/" target="_blank">http://www.mozilla.com/en-US/firefox/new/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp; - Google Chrome : <a href="http://www.google.com/chrome/" target="_blank">http://www.google.com/chrome/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp; - Safari : <a href="http://www.apple.com/safari/" target="_blank">http://www.apple.com/safari/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp; - Opera : <a href="http://www.opera.com/" target="_blank">http://www.opera.com/</a><br />
&nbsp;&nbsp; &nbsp; &nbsp; - Internet Explorer : <a href="http://windows.microsoft.com/en-US/internet-explorer/products/ie/home" target="_blank">http://windows.microsoft.com/en-US/internet-explorer/products/ie/home</a><br />', GETUTCDATE(), N'news', 1, 3, GETUTCDATE(), 3, N'minorUpdates1')
GO