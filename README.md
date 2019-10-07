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
