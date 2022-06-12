USE [moos]

ALTER TABLE os_Cruise
ADD IsLock bit;
ALTER TABLE os_Cruise
ADD LockType nvarchar(50);
ALTER TABLE os_Cruise
ADD LockFromDate datetime;
ALTER TABLE os_Cruise
ADD LockToDate datetime;

CREATE TABLE [dbo].[os_facilities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FacilitieType] [nvarchar](50) NULL,
	[ParentId] [int] NULL,
	[Name] [nvarchar](250) NULL,	
	[IconUrl] [nvarchar](250) NULL,
	[IsMostPopular] [bit] NULL,
 CONSTRAINT [PK_os_facilities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[os_FacilitieMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FacilitieType] [nvarchar](50) NULL,
	[FacilitieId] [int] NULL,
	[ObjectId] [nvarchar](50) NULL,
 CONSTRAINT [PK_os_FacilitieMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[os_ImageGallery]    Script Date: 9/9/2020 9:51:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[os_ImageGallery](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[ImageType] [nvarchar](50) NULL,
	[ImageUrl] [nvarchar](250) NULL,
	[ObjectId] [nvarchar](50) NULL,
 CONSTRAINT [PK_os_ImageGallery] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[os_Reviews]    Script Date: 9/9/2020 9:51:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[os_Reviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReviewType] [nvarchar](50) NULL,
	[UserId] [int] NULL,
	[Body] [nvarchar](2000) NULL,
	[FullName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[Phone] [nvarchar](250) NULL,
	[Disable] [bit] NULL,
	[CreateDate] [datetime] NULL,
	[Rating] [float] NULL,
	[ObjectId] [nvarchar](50) NULL,
 CONSTRAINT [PK_os_Reviews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/************************/
ALTER TABLE os_DocumentCategory
ADD DocumentType nvarchar(50);
ALTER TABLE os_DocumentCategory
ADD ObjectId nvarchar(50);
/************************/
ALTER TABLE os_SailsTrip
ADD IsLock bit;
ALTER TABLE os_SailsTrip
ADD LockType nvarchar(50);
ALTER TABLE os_SailsTrip
ADD LockFromDate datetime;
ALTER TABLE os_SailsTrip
ADD LockToDate datetime;
/****** Object:  Table [dbo].[os_CruiseConfigPrice]    Script Date: 9/27/2020 8:59:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[os_CruiseConfigPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TripConfigPriceId] [int] NULL,
	[CruiseId] [int] NULL,
	[RoomClassId] [int] NULL,
	[RoomTypeId] [int] NULL,
	[Price] [money] NULL,
	[CusFrom] [int] NULL,
	[CusTo] [int] NULL,
	[IsCharter] [bit] NULL,
 CONSTRAINT [PK_os_CruiseConfigPrice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[os_tripConfigPrice]    Script Date: 9/27/2020 9:00:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[os_tripConfigPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TripId] [int] NULL,
	[AgentLevelId] [int] NULL,
	[CampaignId] [int] NULL,
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifyDate] [datetime] NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_os_tripConfigPrice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/************************/
ALTER TABLE os_Campaign
ADD VoucherCode nvarchar(50);
ALTER TABLE os_Campaign
ADD VoucherTotal  [int] NULL;