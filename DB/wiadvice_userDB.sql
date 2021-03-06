USE [master]
GO
/****** Object:  Database [wiadvice_userDB]    Script Date: 24.1.2015 г. 17:16:07 ******/
CREATE DATABASE [wiadvice_userDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'wiadvice_userDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_userDB.mdf' , SIZE = 7360KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'wiadvice_userDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\wiadvice_userDB.ldf' , SIZE = 5824KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [wiadvice_userDB] SET COMPATIBILITY_LEVEL = 90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [wiadvice_userDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [wiadvice_userDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [wiadvice_userDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_userDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [wiadvice_userDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [wiadvice_userDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [wiadvice_userDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [wiadvice_userDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [wiadvice_userDB] SET  MULTI_USER 
GO
ALTER DATABASE [wiadvice_userDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [wiadvice_userDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [wiadvice_userDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [wiadvice_userDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [wiadvice_userDB]
GO
/****** Object:  User [wiadvice_UsersAdmin]    Script Date: 24.1.2015 г. 17:16:07 ******/
CREATE USER [wiadvice_UsersAdmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[wiadvice_UsersAdmin]
GO
/****** Object:  User [wiadvice_EnAdmin]    Script Date: 24.1.2015 г. 17:16:08 ******/
CREATE USER [wiadvice_EnAdmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[wiadvice_EnAdmin]
GO
/****** Object:  User [wiadvice_BgAdmin]    Script Date: 24.1.2015 г. 17:16:08 ******/
CREATE USER [wiadvice_BgAdmin] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[wiadvice_BgAdmin]
GO
ALTER ROLE [db_owner] ADD MEMBER [wiadvice_UsersAdmin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [wiadvice_EnAdmin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [wiadvice_BgAdmin]
GO
/****** Object:  Schema [wiadvice_BgAdmin]    Script Date: 24.1.2015 г. 17:16:08 ******/
CREATE SCHEMA [wiadvice_BgAdmin]
GO
/****** Object:  Schema [wiadvice_EnAdmin]    Script Date: 24.1.2015 г. 17:16:08 ******/
CREATE SCHEMA [wiadvice_EnAdmin]
GO
/****** Object:  Schema [wiadvice_UsersAdmin]    Script Date: 24.1.2015 г. 17:16:08 ******/
CREATE SCHEMA [wiadvice_UsersAdmin]
GO

/****** Object:  StoredProcedure [dbo].[DeleteOldIPAttempts]    Script Date: 24.1.2015 г. 17:16:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteOldIPAttempts] 
AS
BEGIN
	DECLARE @now DATETIME
	DECLARE @oneDayEarlier DATETIME
	DECLARE @twoDaysEarlier DATETIME
	DECLARE @errorCode INT
	
	SET @now = GETUTCDATE()
	SET @oneDayEarlier = DATEADD(day, -1, @now)
	SET @twoDaysEarlier = DATEADD(day, -1, @oneDayEarlier)
	
	BEGIN TRANSACTION
		DELETE FROM [dbo].[IpAttempts]
		WHERE (ISNULL(lastLogIn, @twoDaysEarlier) <= @oneDayEarlier)
			AND (ISNULL(lastAnsSecQuestTry, @twoDaysEarlier) <= @oneDayEarlier)
			AND (ISNULL(lastAnsUserAndMailTry, @twoDaysEarlier) <= @oneDayEarlier)
		SET @errorCode = @@ERROR
		IF (@errorCode != 0)
		BEGIN
			ROLLBACK TRANSACTION
			RAISERROR('Could not delete old IP attempts. Error: %d', 16, 1, @errorCode)
			RETURN
		END
	COMMIT TRANSACTION
	
	SELECT TOP 1 * FROM [dbo].[IpAttempts]
	WHERE [IPaddress] IS NULL
	
END

GO
/****** Object:  StoredProcedure [dbo].[FindCountUsers]    Script Date: 24.1.2015 г. 17:16:08 ******/
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
/****** Object:  StoredProcedure [dbo].[FindUsers]    Script Date: 24.1.2015 г. 17:16:08 ******/
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
/****** Object:  StoredProcedure [dbo].[GetLastDeletedUsers]    Script Date: 24.1.2015 г. 17:16:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLastDeletedUsers]
	@nameContains NVARCHAR(100),
	@count BIGINT,
	@userID BIGINT
AS
BEGIN

	DECLARE @errMsgFmt VARCHAR(200)
	DECLARE @userCriterion NVARCHAR(150)
	DECLARE @sql NVARCHAR(1000)
	
	SET @errMsgFmt = '%s parameter must not be NULL.'
	
	IF (@nameContains IS NOT NULL AND @userID IS NOT NULL)
	BEGIN
		RAISERROR('nameContains is not null AND userID is not null', 16, 1)
		RETURN
	END
	IF (@count IS NULL)
	BEGIN
		RAISERROR(@errMsgFmt, 16, 1, '@count')
		RETURN
	END
	

	IF(@nameContains IS NOT NULL)
	BEGIN
		SET @userCriterion = 'u.username LIKE ''%' + @nameContains + '%'' AND'
	END
	ELSE IF(@userID IS NOT NULL)
	BEGIN
		SET @userCriterion = 'u.ID = ' + CAST(@userID AS NVARCHAR) + ' AND'
	END
	ELSE
	BEGIN
		SET @userCriterion = ' '
	END


	SET @sql = 
	'SELECT TOP ' + CAST(@count AS NVARCHAR) + ' * FROM [dbo].[Users] u' +
	' WHERE ' + @userCriterion + ' u.visible = 0 ' +
	' ORDER BY u.ID DESC'
	EXECUTE (@sql)
	--SELECT @sql
	
END

GO
/****** Object:  StoredProcedure [dbo].[InternalFindUsers]    Script Date: 24.1.2015 г. 17:16:08 ******/
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
			
			INSERT INTO @localUsers(lcID, lcName, lcHits) SELECT ID, username, 0 FROM [dbo].[Users] WHERE [visible] = 1 AND ( [type] = 'user' OR [type] = 'writer')

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
			FROM [dbo].[Users] u, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND u.[ID] = ih.[ihID]
	END
	ELSE
	BEGIN
	SELECT u.* FROM [dbo].[Users] u, @IdsAndHits ih WHERE ih.[ihHits] > 0 AND u.[ID] = ih.[ihID] ORDER BY ih.[ihHits] DESC, u.[username]
	END
END

GO
/****** Object:  UserDefinedFunction [dbo].[AreProductRelationsOk]    Script Date: 24.1.2015 г. 17:16:08 ******/
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
/****** Object:  UserDefinedFunction [dbo].[GetCommentRating]    Script Date: 24.1.2015 г. 17:16:08 ******/
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

/****** Object:  UserDefinedFunction [dbo].[StringSplit]    Script Date: 24.1.2015 г. 17:16:08 ******/
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
/****** Object:  Table [dbo].[Actions]    Script Date: 24.1.2015 г. 17:16:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Actions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [nvarchar](1000) NULL,
	[name] [nvarchar](100) NOT NULL,
	[type] [nvarchar](100) NULL,
	[typeID] [bigint] NULL,
 CONSTRAINT [PK_Actions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IpAttempts]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IpAttempts](
	[IPaddress] [nvarchar](100) NOT NULL,
	[lastLogIn] [datetime] NULL,
	[loginAttempts] [int] NULL,
	[lastAnsSecQuestTry] [datetime] NULL,
	[ansSecQuestAttempts] [int] NULL,
	[lastAnsUserAndMailTry] [datetime] NULL,
	[ansUserAndMailAttempts] [int] NULL,
 CONSTRAINT [PK_IpAttempts] PRIMARY KEY CLUSTERED 
(
	[IPaddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IpBans]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IpBans](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[IPadress] [nvarchar](100) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[byUser] [bigint] NOT NULL,
	[active] [bit] NOT NULL,
	[untillDate] [datetime] NULL,
	[notes] [nvarchar](max) NULL,
	[lastModified] [datetime] NOT NULL,
	[modifiedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_IpBans] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Messages]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[fromUser] [bigint] NOT NULL,
	[toUser] [bigint] NOT NULL,
	[subject] [nvarchar](500) NULL,
	[description] [ntext] NOT NULL,
	[visibleFromUser] [bit] NOT NULL,
	[visibleToUser] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ScalarValues]    Script Date: 24.1.2015 г. 17:16:09 ******/
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
/****** Object:  Table [dbo].[SystemMessages]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemMessages](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[User] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [ntext] NOT NULL,
 CONSTRAINT [PK_SystemMessages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_actions]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_actions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userID] [bigint] NOT NULL,
	[actionID] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[approvedBy] [bigint] NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_user_actions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserLogs]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserModifying] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[description] [ntext] NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[UserModified] [bigint] NOT NULL,
	[IpAdress] [nvarchar](100) NOT NULL,
	[typeModified] [nchar](100) NOT NULL,
 CONSTRAINT [PK_UserLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserOptions]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOptions](
	[userID] [bigint] NOT NULL,
	[canReceiveMessages] [bit] NOT NULL,
	[haveNewMessages] [bit] NOT NULL,
	[secretQuestion] [nvarchar](500) NOT NULL,
	[secretAnswer] [nvarchar](500) NOT NULL,
	[activated] [bit] NOT NULL,
	[activationCode] [nvarchar](1000) NOT NULL,
	[resetPasswordKey] [nvarchar](1000) NULL,
	[dateResetPasswordKeyCreated] [datetime] NULL,
	[haveNewSystemMessages] [bit] NOT NULL,
	[haveNewWarning] [bit] NOT NULL,
	[warnings] [int] NOT NULL,
	[unreadReportReply] [bit] NOT NULL,
	[unseenTypeSuggestionData] [bit] NOT NULL,
	[changeName] [bit] NOT NULL,
	[registeredWithMail] [nvarchar](200) NULL,
 CONSTRAINT [PK_UserOptions] PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](100) NOT NULL,
	[password] [nvarchar](200) NOT NULL,
	[email] [varchar](50) NULL,
	[dateCreated] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
	[type] [nvarchar](100) NOT NULL,
	[createdBy] [bigint] NOT NULL,
	[userData] [ntext] NULL,
	[rating] [float] NOT NULL,
	[lastLogIn] [datetime] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UsersBlocked]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersBlocked](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userBlocking] [bigint] NOT NULL,
	[userBlocked] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[blockActive] [bit] NOT NULL,
 CONSTRAINT [PK_UsersBlocked] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Warnings]    Script Date: 24.1.2015 г. 17:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warnings](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[User] [bigint] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[visible] [bit] NOT NULL,
	[UserAction] [bigint] NULL,
	[description] [ntext] NOT NULL,
	[ByAdmin] [bigint] NOT NULL,
	[lastModified] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NOT NULL,
	[modifiedReason] [ntext] NULL,
 CONSTRAINT [PK_Warnings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Idx_u_Actions_Name]    Script Date: 24.1.2015 г. 17:16:09 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Idx_u_Actions_Name] ON [dbo].[Actions]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Idx_Users_username]    Script Date: 24.1.2015 г. 17:16:09 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Idx_Users_username] ON [dbo].[Users]
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[user_actions] ADD  CONSTRAINT [DF_user_actions_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_visible]  DEFAULT ((1)) FOR [visible]
GO
ALTER TABLE [dbo].[IpBans]  WITH CHECK ADD  CONSTRAINT [FK_IpBans_ByUser] FOREIGN KEY([byUser])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[IpBans] CHECK CONSTRAINT [FK_IpBans_ByUser]
GO
ALTER TABLE [dbo].[IpBans]  WITH CHECK ADD  CONSTRAINT [FK_IpBans_modifiedBy] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[IpBans] CHECK CONSTRAINT [FK_IpBans_modifiedBy]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_FromUser] FOREIGN KEY([fromUser])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_FromUser]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_ToUser] FOREIGN KEY([toUser])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_ToUser]
GO
ALTER TABLE [dbo].[SystemMessages]  WITH CHECK ADD  CONSTRAINT [FK_SystemMessages_User] FOREIGN KEY([User])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[SystemMessages] CHECK CONSTRAINT [FK_SystemMessages_User]
GO
ALTER TABLE [dbo].[user_actions]  WITH CHECK ADD  CONSTRAINT [FK_user-actions_actions] FOREIGN KEY([actionID])
REFERENCES [dbo].[Actions] ([ID])
GO
ALTER TABLE [dbo].[user_actions] CHECK CONSTRAINT [FK_user-actions_actions]
GO
ALTER TABLE [dbo].[user_actions]  WITH CHECK ADD  CONSTRAINT [FK_user-actions1] FOREIGN KEY([approvedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[user_actions] CHECK CONSTRAINT [FK_user-actions1]
GO
ALTER TABLE [dbo].[user_actions]  WITH CHECK ADD  CONSTRAINT [FK_user-actions2] FOREIGN KEY([userID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[user_actions] CHECK CONSTRAINT [FK_user-actions2]
GO
ALTER TABLE [dbo].[UserLogs]  WITH CHECK ADD  CONSTRAINT [FK_UserLogs_UserModified] FOREIGN KEY([UserModified])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UserLogs] CHECK CONSTRAINT [FK_UserLogs_UserModified]
GO
ALTER TABLE [dbo].[UserLogs]  WITH CHECK ADD  CONSTRAINT [FK_UserLogs_UserModifying] FOREIGN KEY([UserModifying])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UserLogs] CHECK CONSTRAINT [FK_UserLogs_UserModifying]
GO
ALTER TABLE [dbo].[UserOptions]  WITH CHECK ADD  CONSTRAINT [FK_UserOptions_User] FOREIGN KEY([userID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UserOptions] CHECK CONSTRAINT [FK_UserOptions_User]
GO
ALTER TABLE [dbo].[UsersBlocked]  WITH CHECK ADD  CONSTRAINT [FK_UsersBlocked_UserBlocked] FOREIGN KEY([userBlocked])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UsersBlocked] CHECK CONSTRAINT [FK_UsersBlocked_UserBlocked]
GO
ALTER TABLE [dbo].[UsersBlocked]  WITH CHECK ADD  CONSTRAINT [FK_UsersBlocked_UserBlocking] FOREIGN KEY([userBlocking])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[UsersBlocked] CHECK CONSTRAINT [FK_UsersBlocked_UserBlocking]
GO
ALTER TABLE [dbo].[Warnings]  WITH CHECK ADD  CONSTRAINT [FK_Warnings_ByAdmin] FOREIGN KEY([ByAdmin])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Warnings] CHECK CONSTRAINT [FK_Warnings_ByAdmin]
GO
ALTER TABLE [dbo].[Warnings]  WITH CHECK ADD  CONSTRAINT [FK_Warnings_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Warnings] CHECK CONSTRAINT [FK_Warnings_ModifiedBy]
GO
ALTER TABLE [dbo].[Warnings]  WITH CHECK ADD  CONSTRAINT [FK_Warnings_User] FOREIGN KEY([User])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Warnings] CHECK CONSTRAINT [FK_Warnings_User]
GO
ALTER TABLE [dbo].[Warnings]  WITH CHECK ADD  CONSTRAINT [FK_Warnings_UserAction] FOREIGN KEY([UserAction])
REFERENCES [dbo].[user_actions] ([ID])
GO
ALTER TABLE [dbo].[Warnings] CHECK CONSTRAINT [FK_Warnings_UserAction]
GO
USE [master]
GO
ALTER DATABASE [wiadvice_userDB] SET  READ_WRITE 
GO

USE [wiadvice_userDB]
GO

SET IDENTITY_INSERT [dbo].[Actions] ON 
GO
INSERT [dbo].[Actions] ([ID], [dateCreated], [description], [name], [type], [typeID]) 
VALUES 
(1, GETUTCDATE(), N'role for writing comments', N'commenter', NULL, NULL)
,(2, GETUTCDATE(), N'role for adding products', N'product', NULL, NULL)
,(3, GETUTCDATE(), N'role for adding companies', N'company', NULL, NULL)
,(4, GETUTCDATE(), N'role for rating a product', N'prater', NULL, NULL)
,(5, GETUTCDATE(), N'role for rating a comment', N'commrater', NULL, NULL)
,(6, GETUTCDATE(), N'role for rating user', N'userrater', NULL, NULL)
,(7, GETUTCDATE(), N'role for flagging comment or product or other stuff for inapporpriate', N'flagger', NULL, NULL)
,(8, GETUTCDATE(), N'role for writing suggestions to the site support or staff', N'suggestor', NULL, NULL)
,(9, GETUTCDATE(), N'role for writing a signature   .. NOT USED...', N'signature', NULL, NULL)
,(10, GETUTCDATE(), N'reserved for future', N'reserved1', NULL, NULL)
,(11, GETUTCDATE(), N'reserved for future', N'reserved2', NULL, NULL)
,(12, GETUTCDATE(), N'reserved for future', N'reserved3', NULL, NULL)
,(13, GETUTCDATE(), N'reserved for future', N'reserved4', NULL, NULL)
,(14, GETUTCDATE(), N'role for creating/editing global administrators', N'gCreator', NULL, NULL)
,(15, GETUTCDATE(), N'role for creating/editing administrators', N'aCreator', NULL, NULL)
,(16, GETUTCDATE(), N'role for creating/editing moderators', N'mCreator', NULL, NULL)
,(17, GETUTCDATE(), N'role for creating/editing categories', N'category', NULL, NULL)
,(18, GETUTCDATE(), N'role for editing all companies', N'acompanies', NULL, NULL)
,(19, GETUTCDATE(), N'role for editing all products', N'aproducts', NULL, NULL)
,(20, GETUTCDATE(), N'role for editing all comments', N'acomments', NULL, NULL)
,(21, GETUTCDATE(), N'role for editing all users/writers', N'ueditor', NULL, NULL)
,(22, GETUTCDATE(), N'reserved for future', N'reserved13', NULL, NULL)
,(23, GETUTCDATE(), N'reserved for future', N'reserved14', NULL, NULL)
,(24, GETUTCDATE(), N'reserved for future', N'reserved15', NULL, NULL)
,(25, GETUTCDATE(), N'reserved for future', N'reserved16', NULL, NULL)
,(26, GETUTCDATE(), N'reserved for future', N'reserved17', NULL, NULL)
,(27, GETUTCDATE(), N'reserved for future', N'reserved18', NULL, NULL)
,(28, GETUTCDATE(), N'reserved for future', N'reserved19', NULL, NULL)
,(29, GETUTCDATE(), N'reserved for future', N'reserved20', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Actions] OFF
GO

INSERT INTO [dbo].[Users] ([username],[password],[email],[dateCreated],[visible],[type],[createdBy],[userData],[rating],[lastLogIn])
     VALUES ('system','765B940C498E709860D783CBDEF660424557399A750231755F7E4BD906689AAC757639DFE401F5020EE2346AB5278B9F5CD772B1',NULL,GETUTCDATE(),1,'system',0,NULL,0,GETUTCDATE())
	 , ('guest','FA9ADD3298AF5301484FB1C0265BBC48339FC2D15BF6DB36342AD74524F553B734DA56F29FE1917D3C3694E480154E83D5E915A2',NULL,GETUTCDATE(),1,'system',1,NULL,0,GETUTCDATE())
	 , ('admin','219C2A62B40E621D550CB59AB9D00C34C9696A5FF72FC35AD1A4E129C1338ABAD19E090C6336DEC71CD2ED160CB9FF9DDF3D302A',NULL,GETUTCDATE(),1,'global',1,NULL,0,GETUTCDATE())
GO

INSERT [dbo].[UserOptions] ([userID], [canReceiveMessages], [haveNewMessages], [secretQuestion], [secretAnswer], [activated], [activationCode], [resetPasswordKey], [dateResetPasswordKeyCreated], [haveNewSystemMessages], [haveNewWarning], [warnings], [unreadReportReply], [unseenTypeSuggestionData], [changeName], [registeredWithMail]) 
VALUES 
(1, 0, 0, N'1', N'1', 1, N'not used', NULL, NULL, 0, 0, 0, 0, 0, 0, NULL)
,(2, 0, 0, N'1', N'1', 1, N'not used', NULL, NULL, 0, 0, 0, 0, 0, 0, NULL)
,(3, 1, 0, N'1', N'1', 1, N'none', NULL, NULL, 0, 0, 0, 0, 0, 0, NULL)
GO

SET IDENTITY_INSERT [dbo].[user_actions] ON 
GO
INSERT [dbo].[user_actions] ([ID], [userID], [actionID], [dateCreated], [approvedBy], [visible]) 
VALUES 
(1, 3, 14, GETUTCDATE(), 1, 1)
,(2, 3, 15, GETUTCDATE(), 1, 1)
,(3, 3, 16, GETUTCDATE(), 1, 1)
,(4, 3, 17, GETUTCDATE(), 1, 1)
,(5, 3, 18, GETUTCDATE(), 1, 1)
,(6, 3, 19, GETUTCDATE(), 1, 1)
,(7, 3, 20, GETUTCDATE(), 1, 1)
,(8, 3, 21, GETUTCDATE(), 1, 1)
GO
SET IDENTITY_INSERT [dbo].[user_actions] OFF
GO



