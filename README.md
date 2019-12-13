# VirtoCommerce.Content

## Overview

VirtoCommerce.Content module is a  Content Management System.

![Content Module](docs/media/screen-content-module.png)

## Key features

1. Themes Management on UI;
1. Pages and blogs management on UI;
1. link lists management;
1. Manage documents in markdown format.

The VC Content module can be accessed both ways: directly and via Stores module.

To access the VC Content directly you should navigate to More->Content Module

![Direct Access](docs/media/screen-access-content-directly.png)

To access the VC Content Module via Stores module, you should navigate to More->Stores->select a Store-> Content module widget

![Access from Store Module](docs/media/screen-access-from-store-module.png)

## Feature Description

### Virto Commerce Theme

Each theme template comes with its own theme settings. These settings allow you to customize your ecommerce website's design without requiring HTML or CSS knowledge.

Change logo, fonts, colors and more. Make your ecommerce website have the look you like.

You can also customize homepage and product layouts, add banners, ads, slideshows and more.

Your ecommerce website can be displayed in any language you want. You can allow customers to pick a language they want or auto detect based on location. Virto Commerce supports local currencies, tax rates, prices etc.

You get full control over all the ecommerce templates and can translate them to any language. If page doesn’t have any translation available then default one can be used instead.

Depending on the theme used, the following translations are available: Danish, Dutch, Spanish, French, German, Greek, Italian, Japanese, Portuguese, Chinese, Swedish and over 50 other languages.

Virto Commerce gives you 100% control over the design and HTML/CSS used by your ecommerce website and online shopping cart.

Virto Commerce utilizes template language called Liquid. Liquid makes it easy to turn your HTML & CSS into a beautiful ecommerce website design.

You can edit templates right in Virto Commerce Manager or using any other editor that you want. Templates can be saved in file system, database or remote repository (github) in order to be able to track all changes.

[Theme Management](/docs/theme-management.md)

### Pages and Blogs

Virto Commerce implements a variation of so called NO-CMS approach (similar to Jekyll) for pages and blogs (and themes to some extent). That means there is no hard dependency on where CMS content like pages, articles and templates are stored. They can be stored in database, github or local file system. This can be configured using CMS config. The content is downloaded from the remote location at runtime and is saved in the local website folder structure under App_Data folder from where local runtime generator picks it up and renders as html.

Pages and blog articles are created using templating engine that supports both markdown and liquid. During runtime those templates are converted to html and persisted in memory until files are changed at which point content is regenerated.

[Pages and Blogs Management](/docs/pages-blogs-management.md)

### Link lists

To view the link lists related to a specific Store, the admin should select the Store and click on the 'Link lists' widget. The system will open the 'Link lists' blade.

The list contains all the links displayed on Storefront, example 'Footer', 'Main menu', etc. The admin can add additional links which will be displayed on the Storefront and edit the existing ones.

The Main menu and Footer are hard-coded. Any other links can be added by the admin, but they should relate to one of the hard coded link.

![Links list](docs/media/screen-link-lists.png)

![Main menu](docs/media/screen-main-menu-link.png)

The system allows to create a specific Main menu for each language.

## Installation

Installing the module:
* Automatically: in VC Manager go to Configuration -> Modules -> CMS Content module -> Install

* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-content/releases. In VC Manager go to Configuration -> Modules -> Advanced -> upload module package -> Install.

## Settings

**VirtoCommerce.Content.CmsContentConnectionString** - CMS content connection string. Defines the provider and connection parameters to connect to content assets (themes, pages, etc.). In fact, it's Platform's <a href="https://virtocommerce.com/docs/vc2devguide/deployment/platform-settings" target="_blank">AssetsConnectionString</a> preconfigured for CMS.

## Available resources

* Module related service implementations as a <a href="https://www.nuget.org/packages/VirtoCommerce.ContentModule.Data" target="_blank">NuGet package</a>
* API client as a <a href="https://www.nuget.org/packages/VirtoCommerce.ContentModule.Client" target="_blank">NuGet package</a>
* API client documentation http://demo.virtocommerce.com/admin/docs/ui/index#!/CMS_Content_module

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
