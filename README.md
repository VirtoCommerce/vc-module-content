# VirtoCommerce.Content
VirtoCommerce.Content module represents CMS (content management system).
Key features:
* themes management in UI
* pages and blogs
* link lists
* documents in markdown format


![Content UI](https://cloud.githubusercontent.com/assets/5801549/15569963/fcbb7336-2333-11e6-808c-c484bf3a7d37.png)

# Documentation
Developer guide:
* <a href="http://docs.virtocommerce.com/x/DQDr" target="_blank">Theme development</a>
* <a href="http://docs.virtocommerce.com/x/joD-/" target="_blank">Pages and Blogs</a>

# Installation
Installing the module:
* Automatically: in VC Manager go to Configuration -> Modules -> CMS Content module -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-content/releases. In VC Manager go to Configuration -> Modules -> Advanced -> upload module package -> Install.

# Settings
* **VirtoCommerce.Content.CmsContentConnectionString** - CMS content connection string. Defines the provider and connection parameters to connect to content assets (themes, pages, etc.). In fact, it's Platform's <a href="http://docs.virtocommerce.com/display/vc2devguide/Platform+settings#Platformsettings-AssetsConnectionString" target="_blank">AssetsConnectionString</a> preconfigured for CMS.

# Available resources
* Module related service implementations as a <a href="https://www.nuget.org/packages/VirtoCommerce.ContentModule.Data" target="_blank">NuGet package</a>
* API client as a <a href="https://www.nuget.org/packages/VirtoCommerce.ContentModule.Client" target="_blank">NuGet package</a>
* API client documentation http://demo.virtocommerce.com/admin/docs/ui/index#!/CMS_Content_module

# License
Copyright (c) Virtosoftware Ltd.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
