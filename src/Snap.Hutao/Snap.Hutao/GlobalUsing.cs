// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

// CommunityToolkit
global using CommunityToolkit.Mvvm.DependencyInjection;
global using CommunityToolkit.Mvvm.Messaging;

// Microsoft
global using Microsoft;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

// Sentry
global using Sentry;

// Snap.Hutao
global using Snap.Hutao.Core.Annotation;
global using Snap.Hutao.Core.DependencyInjection;
global using Snap.Hutao.Core.DependencyInjection.Annotation;
global using Snap.Hutao.Core.Threading;
global using Snap.Hutao.Extension;
global using Snap.Hutao.Resource.Localization;

// Runtime
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Input;

// Snap.Hutao
global using FromKeyed = Snap.Hutao.Core.DependencyInjection.Annotation.FromKeyedServicesAttribute;
global using Void = Snap.Hutao.Core.Void;