# GeoNames2Sql

GeoNames2Sql is a tool built by the Gaois research group at [Fiontar & Scoil na Gaeilge](https://www.gaois.ie), Dublin City University, Ireland to import Gazetteer data from the [GeoNames geographical database](http://www.geonames.org/) and store it in a SQL Server instance. It is essentially an abstraction over the excellent [NGeoNames](https://github.com/RobThree/NGeoNames) library, adding a Command-line Interface and the tools to generate a SQL database. GeoNames2Sql is implemented as a .NET Core 2.1 Console Application, meaning it can run cross-platform. This implementation presumes a SQL Server data store but it should be relatively trivial to implement providers for PostgreSQL or other relational database types.

## Status

This application addresses the minimum requirements of the Gaois research group. It is not, as yet, comprehensive for all other use cases. The output data set includes tables for (1) GeoNames, (2) alternate names, and (3) country info. There are not yet tables for feature codes, admin codes, time zones and the other reference lists included in the GeoNames data. These items may be included in the future, if the needs arises, and we also welcome PRs from other users.

## Installation and setup

### Database

1. Give the application permissions to a database.
2. Run the [SQL script](https://github.com/gaois/GeoNames2Sql/blob/master/scripts/CreateTables.sql) to create the database tables.
3. Optionally, create indexes for your tables. Some sample indexes are provided [here](https://github.com/gaois/GeoNames2Sql/blob/master/scripts/CreateIndexes.sql).

The database schema mirrors the GeoNames Gazetteer data structure described [here](http://download.geonames.org/export/dump/).

### Application

Clone the repository to your machine:

```cmd
git clone https://github.com/gaois/GeoNames2Sql
```

Then, build the .NET Core solution, specifying your target runtime environment, e.g.:

```cmd
dotnet build -r win10-x64
```

## Usage

**Tip:** See a list of target runtime identifiers [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog?irgwc=1&OCID=AID681541_aff_7593_1243925&tduid=(ir_6d4f9ce9N213458eb7517c20a2b9db916)(7593)(1243925)(je6NUbpObpQ-wDYfcuMFmHDb6Ja3HC_Ryw)()&irclickid=6d4f9ce9N213458eb7517c20a2b9db916#using-rids?ranMID=24542&ranEAID=je6NUbpObpQ&ranSiteID=je6NUbpObpQ-wDYfcuMFmHDb6Ja3HC_Ryw&epi=je6NUbpObpQ-wDYfcuMFmHDb6Ja3HC_Ryw).

This will output a collection of dynamic linked libraries (.dll files) and an appsettings.json file that you can grab from `<PATH-TO-YOUR-APP>/bin/Debug/netcoreapp2.0/`. Or else, you can just run the application from within Visual Studio.
