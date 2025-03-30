# Dependency Injection

## Overview

This document describes the dependency injection (DI) system used in the project.

## Design Remarks

### Service Scope Notice

* Generally avoid Singleton services.
* All `Window`s should be transient themselves to allow multiple times instantiation.
  * Window creates a scope for its lifetime and should be disposed of when closed.
  * Window content as xaml element can't be resolved from the service provider. **But the view model associated with the content can and should be resolved from the window scope.**
* Normal ViewModel should generally be transient. (Heavy view models can be singleton)
* Page ViewModel should be scoped.
* For values that requires stay alive for both xaml binding and code access, place them in a singleton Options class.
  * This is an anti-pattern, but currently it's the best solution.