﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

/// <summary>
/// Specifies the way in which a set of elements (e.g. resultset, collection, etc.) must be sorted.
/// </summary>
public enum SortOptions
{
    /// <summary>
    /// No sort is explicitly applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Sort in ascending order (smallest goes first).
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// Sort in descending order (biggest goes first).
    /// </summary>
    Descending = 2
}


public enum MailKind
{
    UserActivationLink,
    ResetPasswordLink,
    UserNewPassword,
    ToSite
}

public enum CommentType
{
    Product,
    Topic
}

public enum CommentSubType
{
    Comment,
    SubComment
}

public enum SuggestionType
{
    General,
    Design,
    Features
}

public enum UserRoles
{
    AddProducts,
    AddCompanies,
    WriteCommentsAndMessages,
    ReportInappropriate,
    RateProducts,
    RateUsers,
    RateComments,
    WriteSuggestions,
    HaveSignature
}

public enum AdminRoles
{
    EditGlobalAdministrators,
    EditAdministrators,
    EditModerators,
    EditCategories,
    EditCompanies,
    EditProducts,
    EditUsers,
    EditComments
}

public enum UserTypes
{
    User,
    Writer,
    Moderator,
    Administrator,
    GlobalAdministrator,
    System
}

public enum AdvertisementsFor
{
    Products,
    Companies,
    Categories
}

public enum NotifyType
{
    Product,
    Company,
    ProductForum,
    ProductTopic
}

public enum LogType
{
    create,
    delete,
    undelete,
    edit
}

public enum IpAttemptTry
{
    LogIn,
    AnswerSecQuestion,
    guessUserAndMail
}

public enum VisitedType
{
    ProductTopic
}

public enum ModificationType
{
    ProductTopic
}