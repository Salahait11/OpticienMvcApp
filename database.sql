USE [master]
GO
/****** Object:  Database [OPTICIEN]    Script Date: 10/06/2025 22:57:44 ******/
CREATE DATABASE [OPTICIEN]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'OPTICIEN', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\OPTICIEN.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'OPTICIEN_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\OPTICIEN_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [OPTICIEN] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OPTICIEN].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OPTICIEN] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OPTICIEN] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OPTICIEN] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OPTICIEN] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OPTICIEN] SET ARITHABORT OFF 
GO
ALTER DATABASE [OPTICIEN] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [OPTICIEN] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [OPTICIEN] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [OPTICIEN] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [OPTICIEN] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [OPTICIEN] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [OPTICIEN] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [OPTICIEN] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [OPTICIEN] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [OPTICIEN] SET  ENABLE_BROKER 
GO
ALTER DATABASE [OPTICIEN] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [OPTICIEN] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [OPTICIEN] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [OPTICIEN] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [OPTICIEN] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [OPTICIEN] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [OPTICIEN] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [OPTICIEN] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [OPTICIEN] SET  MULTI_USER 
GO
ALTER DATABASE [OPTICIEN] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [OPTICIEN] SET DB_CHAINING OFF 
GO
ALTER DATABASE [OPTICIEN] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [OPTICIEN] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [OPTICIEN] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [OPTICIEN] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [OPTICIEN] SET QUERY_STORE = ON
GO
ALTER DATABASE [OPTICIEN] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [OPTICIEN]
GO
/****** Object:  Table [dbo].[CategorieProduit]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategorieProduit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nom] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Nom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Client]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nom] [varchar](50) NOT NULL,
	[Prenom] [varchar](50) NOT NULL,
	[DateDeNaissance] [date] NULL,
	[Adresse] [varchar](255) NOT NULL,
	[NumeroDeTelephone] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetailMonture]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetailMonture](
	[ProduitID] [int] NOT NULL,
	[Marque] [varchar](100) NOT NULL,
	[Modele] [varchar](100) NULL,
	[Couleur] [varchar](50) NOT NULL,
	[Taille] [varchar](20) NOT NULL,
	[Materiau] [varchar](50) NULL,
	[Genre] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProduitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetailVerre]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetailVerre](
	[ProduitID] [int] NOT NULL,
	[TypeTraitement] [varchar](255) NULL,
	[Indice] [decimal](3, 2) NULL,
	[DiametrePrecal] [decimal](4, 1) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProduitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LignOpVente]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LignOpVente](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OperationVenteID] [int] NOT NULL,
	[ProduitID] [int] NOT NULL,
	[Quantite] [int] NOT NULL,
	[PrixUnitaireVendu] [decimal](10, 2) NOT NULL,
	[Remise] [decimal](10, 2) NOT NULL,
	[Ligne_OD_SPH] [varchar](10) NULL,
	[Ligne_OD_CYL] [varchar](10) NULL,
	[Ligne_OD_AXE] [varchar](10) NULL,
	[Ligne_OD_ADD] [varchar](10) NULL,
	[Ligne_OG_SPH] [varchar](10) NULL,
	[Ligne_OG_CYL] [varchar](10) NULL,
	[Ligne_OG_AXE] [varchar](10) NULL,
	[Ligne_OG_ADD] [varchar](10) NULL,
	[Ligne_EP_VL] [varchar](10) NULL,
	[Ligne_H_Prog] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medecin]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medecin](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nom] [varchar](50) NOT NULL,
	[Prenom] [varchar](50) NOT NULL,
	[Gsm] [varchar](20) NULL,
	[NumeroRPPS] [varchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationVente]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationVente](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NumeroOp] [varchar](20) NOT NULL,
	[DateDeVente] [datetime] NOT NULL,
	[DateLivrisonPrevu] [datetime] NULL,
	[Remarque] [varchar](500) NULL,
	[ClientID] [int] NOT NULL,
	[OrdID] [int] NULL,
	[VendeurID] [int] NULL,
	[Statut] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[NumeroOp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ordonnance]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ordonnance](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateDePrescription] [date] NOT NULL,
	[DateExpiration] [date] NULL,
	[OD_VL_SPH] [varchar](10) NULL,
	[OD_VL_CYL] [varchar](10) NULL,
	[OD_VL_AXE] [varchar](10) NULL,
	[OD_VL_ADD] [varchar](10) NULL,
	[OD_VL_EP] [varchar](10) NULL,
	[OD_VL_H] [varchar](10) NULL,
	[OD_VL_DAIM] [varchar](10) NULL,
	[OG_VL_SPH] [varchar](10) NULL,
	[OG_VL_CYL] [varchar](10) NULL,
	[OG_VL_AXE] [varchar](10) NULL,
	[OG_VL_ADD] [varchar](10) NULL,
	[OG_VL_EP] [varchar](10) NULL,
	[OG_VL_H] [varchar](10) NULL,
	[OG_VL_DAIM] [varchar](10) NULL,
	[OD_VP_SPH] [varchar](10) NULL,
	[OD_VP_CYL] [varchar](10) NULL,
	[OD_VP_AXE] [varchar](10) NULL,
	[OD_VP_ADD] [varchar](10) NULL,
	[OD_VP_EP] [varchar](10) NULL,
	[OD_VP_H] [varchar](10) NULL,
	[OD_VP_DAIM] [varchar](10) NULL,
	[OG_VP_SPH] [varchar](10) NULL,
	[OG_VP_CYL] [varchar](10) NULL,
	[OG_VP_AXE] [varchar](10) NULL,
	[OG_VP_ADD] [varchar](10) NULL,
	[OG_VP_EP] [varchar](10) NULL,
	[OG_VP_H] [varchar](10) NULL,
	[OG_VP_DAIM] [varchar](10) NULL,
	[MedecinID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PAIEMENT]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAIEMENT](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DatePaiement] [datetime] NOT NULL,
	[ModeDePaiement] [varchar](50) NOT NULL,
	[MontantPaye] [decimal](10, 2) NOT NULL,
	[OpVenteID] [int] NOT NULL,
	[ReferencePaiement] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Produit]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Produit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [varchar](50) NOT NULL,
	[Designation] [varchar](255) NOT NULL,
	[CategorieID] [int] NOT NULL,
	[PrixUnitaire] [decimal](10, 2) NOT NULL,
	[TauxTVA] [decimal](4, 2) NOT NULL,
	[Fabricant] [varchar](100) NULL,
	[Actif] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendeur]    Script Date: 10/06/2025 22:57:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendeur](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nom] [varchar](50) NOT NULL,
	[Prenom] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Client_NomPrenom]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Client_NomPrenom] ON [dbo].[Client]
(
	[Nom] ASC,
	[Prenom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LignOpVente_OperationVenteID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_LignOpVente_OperationVenteID] ON [dbo].[LignOpVente]
(
	[OperationVenteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LignOpVente_ProduitID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_LignOpVente_ProduitID] ON [dbo].[LignOpVente]
(
	[ProduitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Medecin_NomPrenom]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Medecin_NomPrenom] ON [dbo].[Medecin]
(
	[Nom] ASC,
	[Prenom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OperationVente_ClientID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_OperationVente_ClientID] ON [dbo].[OperationVente]
(
	[ClientID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OperationVente_DateDeVente]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_OperationVente_DateDeVente] ON [dbo].[OperationVente]
(
	[DateDeVente] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OperationVente_Statut]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_OperationVente_Statut] ON [dbo].[OperationVente]
(
	[Statut] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Ordonnance_DatePrescription]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Ordonnance_DatePrescription] ON [dbo].[Ordonnance]
(
	[DateDePrescription] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Ordonnance_MedecinID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Ordonnance_MedecinID] ON [dbo].[Ordonnance]
(
	[MedecinID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PAIEMENT_DatePaiement]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_PAIEMENT_DatePaiement] ON [dbo].[PAIEMENT]
(
	[DatePaiement] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PAIEMENT_OpVenteID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_PAIEMENT_OpVenteID] ON [dbo].[PAIEMENT]
(
	[OpVenteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Produit_CategorieID]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Produit_CategorieID] ON [dbo].[Produit]
(
	[CategorieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Produit_Designation]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Produit_Designation] ON [dbo].[Produit]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendeur_NomPrenom]    Script Date: 10/06/2025 22:57:44 ******/
CREATE NONCLUSTERED INDEX [IX_Vendeur_NomPrenom] ON [dbo].[Vendeur]
(
	[Nom] ASC,
	[Prenom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LignOpVente] ADD  DEFAULT ((1)) FOR [Quantite]
GO
ALTER TABLE [dbo].[LignOpVente] ADD  DEFAULT ((0)) FOR [Remise]
GO
ALTER TABLE [dbo].[OperationVente] ADD  DEFAULT (getdate()) FOR [DateDeVente]
GO
ALTER TABLE [dbo].[OperationVente] ADD  DEFAULT ('En cours') FOR [Statut]
GO
ALTER TABLE [dbo].[PAIEMENT] ADD  DEFAULT (getdate()) FOR [DatePaiement]
GO
ALTER TABLE [dbo].[Produit] ADD  DEFAULT ((20.00)) FOR [TauxTVA]
GO
ALTER TABLE [dbo].[Produit] ADD  DEFAULT ((1)) FOR [Actif]
GO
ALTER TABLE [dbo].[DetailMonture]  WITH CHECK ADD  CONSTRAINT [FK_DetailMonture_Produit] FOREIGN KEY([ProduitID])
REFERENCES [dbo].[Produit] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DetailMonture] CHECK CONSTRAINT [FK_DetailMonture_Produit]
GO
ALTER TABLE [dbo].[DetailVerre]  WITH CHECK ADD  CONSTRAINT [FK_DetailVerre_Produit] FOREIGN KEY([ProduitID])
REFERENCES [dbo].[Produit] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DetailVerre] CHECK CONSTRAINT [FK_DetailVerre_Produit]
GO
ALTER TABLE [dbo].[LignOpVente]  WITH CHECK ADD  CONSTRAINT [FK_LignOpVente_OperationVente] FOREIGN KEY([OperationVenteID])
REFERENCES [dbo].[OperationVente] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LignOpVente] CHECK CONSTRAINT [FK_LignOpVente_OperationVente]
GO
ALTER TABLE [dbo].[LignOpVente]  WITH CHECK ADD  CONSTRAINT [FK_LignOpVente_Produit] FOREIGN KEY([ProduitID])
REFERENCES [dbo].[Produit] ([ID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[LignOpVente] CHECK CONSTRAINT [FK_LignOpVente_Produit]
GO
ALTER TABLE [dbo].[OperationVente]  WITH CHECK ADD  CONSTRAINT [FK_OperationVente_Client] FOREIGN KEY([ClientID])
REFERENCES [dbo].[Client] ([ID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[OperationVente] CHECK CONSTRAINT [FK_OperationVente_Client]
GO
ALTER TABLE [dbo].[OperationVente]  WITH CHECK ADD  CONSTRAINT [FK_OperationVente_Ordonnance] FOREIGN KEY([OrdID])
REFERENCES [dbo].[Ordonnance] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[OperationVente] CHECK CONSTRAINT [FK_OperationVente_Ordonnance]
GO
ALTER TABLE [dbo].[OperationVente]  WITH CHECK ADD  CONSTRAINT [FK_OperationVente_Vendeur] FOREIGN KEY([VendeurID])
REFERENCES [dbo].[Vendeur] ([ID])
GO
ALTER TABLE [dbo].[OperationVente] CHECK CONSTRAINT [FK_OperationVente_Vendeur]
GO
ALTER TABLE [dbo].[Ordonnance]  WITH CHECK ADD  CONSTRAINT [FK_Ordonnance_Medecin] FOREIGN KEY([MedecinID])
REFERENCES [dbo].[Medecin] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Ordonnance] CHECK CONSTRAINT [FK_Ordonnance_Medecin]
GO
ALTER TABLE [dbo].[PAIEMENT]  WITH CHECK ADD  CONSTRAINT [FK_PAIEMENT_OperationVente] FOREIGN KEY([OpVenteID])
REFERENCES [dbo].[OperationVente] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PAIEMENT] CHECK CONSTRAINT [FK_PAIEMENT_OperationVente]
GO
ALTER TABLE [dbo].[Produit]  WITH CHECK ADD  CONSTRAINT [FK_Produit_Categorie] FOREIGN KEY([CategorieID])
REFERENCES [dbo].[CategorieProduit] ([ID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Produit] CHECK CONSTRAINT [FK_Produit_Categorie]
GO
ALTER TABLE [dbo].[DetailMonture]  WITH CHECK ADD  CONSTRAINT [CK_DetailMonture_Genre] CHECK  (([Genre]='U' OR [Genre]='F' OR [Genre]='H'))
GO
ALTER TABLE [dbo].[DetailMonture] CHECK CONSTRAINT [CK_DetailMonture_Genre]
GO
ALTER TABLE [dbo].[LignOpVente]  WITH CHECK ADD  CONSTRAINT [CK_LignOpVente_PrixPositif] CHECK  (([PrixUnitaireVendu]>=(0)))
GO
ALTER TABLE [dbo].[LignOpVente] CHECK CONSTRAINT [CK_LignOpVente_PrixPositif]
GO
ALTER TABLE [dbo].[LignOpVente]  WITH CHECK ADD  CONSTRAINT [CK_LignOpVente_QuantitePositive] CHECK  (([Quantite]>(0)))
GO
ALTER TABLE [dbo].[LignOpVente] CHECK CONSTRAINT [CK_LignOpVente_QuantitePositive]
GO
ALTER TABLE [dbo].[LignOpVente]  WITH CHECK ADD  CONSTRAINT [CK_LignOpVente_RemisePositive] CHECK  (([Remise]>=(0)))
GO
ALTER TABLE [dbo].[LignOpVente] CHECK CONSTRAINT [CK_LignOpVente_RemisePositive]
GO
ALTER TABLE [dbo].[OperationVente]  WITH CHECK ADD  CONSTRAINT [CK_OperationVente_Statut] CHECK  (([Statut]='Annulé' OR [Statut]='Payé' OR [Statut]='Livré' OR [Statut]='Prêt' OR [Statut]='Commandé' OR [Statut]='En cours'))
GO
ALTER TABLE [dbo].[OperationVente] CHECK CONSTRAINT [CK_OperationVente_Statut]
GO
ALTER TABLE [dbo].[PAIEMENT]  WITH CHECK ADD  CONSTRAINT [CK_PAIEMENT_MontantPositif] CHECK  (([MontantPaye]>(0)))
GO
ALTER TABLE [dbo].[PAIEMENT] CHECK CONSTRAINT [CK_PAIEMENT_MontantPositif]
GO
ALTER TABLE [dbo].[Produit]  WITH CHECK ADD  CONSTRAINT [CK_Produit_PrixPositif] CHECK  (([PrixUnitaire]>=(0)))
GO
ALTER TABLE [dbo].[Produit] CHECK CONSTRAINT [CK_Produit_PrixPositif]
GO
ALTER TABLE [dbo].[Produit]  WITH CHECK ADD  CONSTRAINT [CK_Produit_TVAPositive] CHECK  (([TauxTVA]>=(0)))
GO
ALTER TABLE [dbo].[Produit] CHECK CONSTRAINT [CK_Produit_TVAPositive]
GO
USE [master]
GO
ALTER DATABASE [OPTICIEN] SET  READ_WRITE 
GO
