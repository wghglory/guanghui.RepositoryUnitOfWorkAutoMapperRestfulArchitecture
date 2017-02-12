Since webapi cannot reference Repository directly, this line of code is not allowed:

container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager()); 

So i create this Resolver project using Unity and MEF to solve it.

add a reference: System.ComponentModel.Composition (MEF)


This is common to all projects, anyone can use. BusinessServices, Repository, Webapi will reference it in this case.