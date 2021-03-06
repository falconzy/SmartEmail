USE [webzyEmail]
GO
/****** Object:  Table [dbo].[EmailLog]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SenderName] [varchar](250) NULL,
	[SenderAddress] [varchar](250) NULL,
	[ReceiverName] [varchar](250) NULL,
	[ReceiverAddress] [varchar](250) NULL,
	[CCAddress] [varchar](1000) NULL,
	[Subject] [varchar](250) NULL,
	[EmailContent] [varchar](8000) NULL,
	[EmailFormat] [varchar](10) NULL,
	[EmailSeverity] [varchar](50) NULL,
	[EmailSent] [bit] NOT NULL,
	[Remarks] [varchar](250) NULL,
	[ModifiedBy] [varchar](250) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_EmailLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[EmailLog] ON
INSERT [dbo].[EmailLog] ([Id], [SenderName], [SenderAddress], [ReceiverName], [ReceiverAddress], [CCAddress], [Subject], [EmailContent], [EmailFormat], [EmailSeverity], [EmailSent], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (1, N'Helpdesk', N'falconzy@gmail.com', N'yan.zhao@nxg-c.com', N'yan.zhao@nxg-c.com', N'', N'test Mail', N'test email.', N'HTML', N'Important', 0, NULL, N'awsemail', CAST(0x0000A20D00FA9167 AS DateTime))
INSERT [dbo].[EmailLog] ([Id], [SenderName], [SenderAddress], [ReceiverName], [ReceiverAddress], [CCAddress], [Subject], [EmailContent], [EmailFormat], [EmailSeverity], [EmailSent], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (2, N'Helpdesk', N'falconzy@gmail.com', N'yan.zhao@nxg-c.com', N'yan.zhao@nxg-c.com', N'', N'test Mail', N'test email.', N'HTML', N'Important', 1, NULL, N'awsemail', CAST(0x0000A20D00FB8860 AS DateTime))
SET IDENTITY_INSERT [dbo].[EmailLog] OFF
/****** Object:  Table [dbo].[Setting]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Setting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SMTP] [varchar](100) NULL,
	[POP3] [varchar](100) NULL,
	[IMAP] [varchar](100) NULL,
	[SaveToServer] [bit] NULL,
	[POPPort] [int] NULL,
	[POPsecurity] [bit] NULL,
	[SMTPPort] [int] NULL,
	[SMTPsecurity] [bit] NULL,
	[IMAPPort] [int] NULL,
	[IMAPsecurity] [bit] NULL,
	[EmailAddress] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[DefaultServer] [bit] NULL,
	[TimerCycle] [int] NULL,
	[Remarks] [varchar](250) NULL,
	[ModifiedBy] [varchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_EmailSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Setting] ON
INSERT [dbo].[Setting] ([Id], [Name], [SMTP], [POP3], [IMAP], [SaveToServer], [POPPort], [POPsecurity], [SMTPPort], [SMTPsecurity], [IMAPPort], [IMAPsecurity], [EmailAddress], [UserId], [Password], [DefaultServer], [TimerCycle], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (1, N'Gmail', N' smtp.gmail.com', N'pod51021.outlook.com', N'pod51021.outlook.com', 0, 465, 1, 587, 1, 993, 1, N'falconzy@gmail.com', N'falconzy@gmail.com', N'840522', 1, 5, N'', N'Zhao Yan ', CAST(0x0000A07C00000000 AS DateTime))
INSERT [dbo].[Setting] ([Id], [Name], [SMTP], [POP3], [IMAP], [SaveToServer], [POPPort], [POPsecurity], [SMTPPort], [SMTPsecurity], [IMAPPort], [IMAPsecurity], [EmailAddress], [UserId], [Password], [DefaultServer], [TimerCycle], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (6, N'AWS', N'email-smtp.us-east-1.amazonaws.com', N' ', N' ', 0, 465, 1, 587, 1, NULL, NULL, N'falconzy@gmail.com', N'AKIAJU72YDBG5HSIP64Q', N'Apgu/qPGP3MjMj8JvZR1Nt2T9qFL2rzXsZIbxoy+pYyc', 1, 5, N' ', N'Zhao Yan', CAST(0x0000A20D00000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[Setting] OFF
/****** Object:  Table [dbo].[ServiceLog]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServiceLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [varchar](500) NULL,
	[EmailSettingId] [int] NULL,
	[ErrorMessage] [varchar](1000) NOT NULL,
	[Action] [varchar](100) NULL,
	[ProcessName] [varchar](500) NULL,
	[Remarks] [varchar](250) NULL,
	[ModifiedBy] [varchar](250) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_ErrorMsgLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[ServiceLog] ON
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (1, NULL, 0, N'Bad Data.
', N'VerifyId', N'VerifyIdentityId', NULL, N'awsemail', CAST(0x0000A20D00F784BC AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (2, N'yan.zhao@nxg-c.com', -1, N'Sorry,User Validation Fail', N'Sending', N'SendEmail', NULL, N'awsemail', CAST(0x0000A20D00F78DBB AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (3, NULL, 0, N'Bad Data.
', N'VerifyId', N'VerifyIdentityId', NULL, N'awsemail', CAST(0x0000A20D00F7F680 AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (4, N'yan.zhao@nxg-c.com', -1, N'Sorry,User Validation Fail', N'Sending', N'SendEmail', NULL, N'awsemail', CAST(0x0000A20D00F7F687 AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (5, NULL, 0, N'Bad Data.
', N'VerifyId', N'VerifyIdentityId', NULL, N'awsemail', CAST(0x0000A20D00F80E97 AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (6, N'yan.zhao@nxg-c.com', -1, N'Sorry,User Validation Fail', N'Sending', N'SendEmail', NULL, N'awsemail', CAST(0x0000A20D00F80E9E AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (7, NULL, 0, N'Bad Data.
', N'VerifyId', N'VerifyIdentityId', NULL, N'awsemail', CAST(0x0000A20D00F8432F AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (8, N'yan.zhao@nxg-c.com', -1, N'Sorry,User Validation Fail', N'Sending', N'SendEmail', NULL, N'awsemail', CAST(0x0000A20D00F84330 AS DateTime))
INSERT [dbo].[ServiceLog] ([Id], [EmailAddress], [EmailSettingId], [ErrorMessage], [Action], [ProcessName], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (9, N'yan.zhao@nxg-c.com', 6, N'The SMTP server requires a secure connection or the client was not authenticated. The server response was: Authentication required', N'Sending', N'SendMail', NULL, N'awsemail', CAST(0x0000A20D00FAC970 AS DateTime))
SET IDENTITY_INSERT [dbo].[ServiceLog] OFF
/****** Object:  Table [dbo].[ServiceAccount]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServiceAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](250) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[IdentityId] [varchar](50) NOT NULL,
	[EmailSettingFk] [int] NOT NULL,
	[Remarks] [varchar](250) NULL,
	[ModifiedBy] [varchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_EmailServiceAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[ServiceAccount] ON
INSERT [dbo].[ServiceAccount] ([Id], [UserName], [Password], [IdentityId], [EmailSettingFk], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (1, N'webzy', N'', N'RWt3BfnFQlA=', 1, N'Gmail Account', N'Zhao Yan', CAST(0x00009F8700000000 AS DateTime))
INSERT [dbo].[ServiceAccount] ([Id], [UserName], [Password], [IdentityId], [EmailSettingFk], [Remarks], [ModifiedBy], [ModifiedOn]) VALUES (4, N'awsemail', N' ', N'hwlM+eGk3i4xH4XlsDmvzw==', 6, N'AWS Email Account', N'Zhao Yan', CAST(0x0000A20D00000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[ServiceAccount] OFF
/****** Object:  Table [dbo].[ReceivedEmail]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceivedEmail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UID] [varchar](5000) NOT NULL,
	[EmailSentTime] [datetime] NOT NULL,
	[EmailAccountFk] [int] NOT NULL,
	[FromAddress] [varchar](500) NOT NULL,
	[ToAddress] [varchar](500) NULL,
	[CCAddress] [varchar](1000) NULL,
	[Subject] [varchar](500) NULL,
	[EmailContent] [varchar](max) NULL,
	[EmailFormat] [varchar](50) NULL,
	[EmailSeverity] [varchar](50) NULL,
	[AttachedFile] [bit] NOT NULL,
	[Readed] [bit] NOT NULL,
	[ModifiedBy] [varchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ReceivedEmail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentFile]    Script Date: 08/01/2013 15:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailReceivedFk] [int] NULL,
	[UID] [varchar](5000) NOT NULL,
	[FileName] [varchar](500) NULL,
	[FileLocation] [varchar](500) NULL,
	[ModifiedBy] [varchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_EmailAttachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[EmailReceivedView]    Script Date: 08/01/2013 15:20:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EmailReceivedView]
AS
SELECT     dbo.ReceivedEmail.Id, dbo.ReceivedEmail.UID, dbo.ReceivedEmail.EmailSentTime, dbo.ReceivedEmail.FromAddress, dbo.ReceivedEmail.ToAddress, 
                      dbo.ReceivedEmail.CCAddress, dbo.ReceivedEmail.Subject, dbo.ReceivedEmail.EmailContent, dbo.ReceivedEmail.EmailSeverity, dbo.ReceivedEmail.AttachedFile, 
                      dbo.ReceivedEmail.Readed, dbo.ReceivedEmail.ModifiedBy, dbo.ReceivedEmail.ModifiedOn, dbo.ServiceAccount.IdentityId
FROM         dbo.ReceivedEmail INNER JOIN
                      dbo.ServiceAccount ON dbo.ReceivedEmail.Id = dbo.ServiceAccount.Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[42] 4[22] 2[10] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ReceivedEmail"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 190
               Right = 201
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "ServiceAccount"
            Begin Extent = 
               Top = 6
               Left = 239
               Bottom = 190
               Right = 399
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 15
         Width = 284
         Width = 2520
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EmailReceivedView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EmailReceivedView'
GO
/****** Object:  ForeignKey [FK_EmailAttachments_EmailReceived]    Script Date: 08/01/2013 15:20:51 ******/
ALTER TABLE [dbo].[AttachmentFile]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_EmailReceived] FOREIGN KEY([EmailReceivedFk])
REFERENCES [dbo].[ReceivedEmail] ([Id])
GO
ALTER TABLE [dbo].[AttachmentFile] CHECK CONSTRAINT [FK_EmailAttachments_EmailReceived]
GO
/****** Object:  ForeignKey [FK_EmailReceived_EmailSetting]    Script Date: 08/01/2013 15:20:51 ******/
ALTER TABLE [dbo].[ReceivedEmail]  WITH CHECK ADD  CONSTRAINT [FK_EmailReceived_EmailSetting] FOREIGN KEY([EmailAccountFk])
REFERENCES [dbo].[Setting] ([Id])
GO
ALTER TABLE [dbo].[ReceivedEmail] CHECK CONSTRAINT [FK_EmailReceived_EmailSetting]
GO
/****** Object:  ForeignKey [FK_EmailServiceAccount_EmailSetting]    Script Date: 08/01/2013 15:20:51 ******/
ALTER TABLE [dbo].[ServiceAccount]  WITH CHECK ADD  CONSTRAINT [FK_EmailServiceAccount_EmailSetting] FOREIGN KEY([EmailSettingFk])
REFERENCES [dbo].[Setting] ([Id])
GO
ALTER TABLE [dbo].[ServiceAccount] CHECK CONSTRAINT [FK_EmailServiceAccount_EmailSetting]
GO
