# WS2021 SWE3 OR Mapper

A object relationship mapper written in c# which was tested with the PostgreSql database.

## Setup Demo

For the examples provided in the project the following requirments are needed:

- Running PostgreSql database
  - On localhost with username postgres and password postgres
  - The connection string used in examples can be found in the BaseTest class
  - A docker-compose is provided for easy startup

## Features

+ Managing Entities (Save, Update, Delete)
+ Quering Entities
+ Custom Query support per Entitiy Lists
+ Uses Repository pattern
+ Annotation based
+ Automatic Table generation
+ Only one field primary keys supported

## Usage

To mark a model class as an entity and configure it attributes need to be used.
<br>
An example:
```csharp
[Entity]
public class Book
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
    [ForeignKey(RemoteTableName = "BookOwnedByCustomer", RemoteTableColumnName = "customerid", ColumnName = "broughtbookid")]
    public List<Customer> OwnedBy { get; set; } = new List<Customer>();
}
```
The available attributes are:
+ Entity - required for classes, marks it as an entity
+ Field - optional for properties to configure additional information
+ Ignore  - optional for properties to make the OR Mapper ignore the property
+ ForeignKey - required for properties representing a foreign key
+ PrimaryKey - required for every entity on one property is the primary key

After adding the attributes to the model, the Repository needs to be created.

```csharp
IDbConnection databaseConnection = new NpgsqlConnection(_connectionString);
databaseConnection.Open();
Repository<Book> bookRepository = new Repository<Book>(_databaseConnection);
```
It is done with a instance of IDBConnection and a optional dictionary which represents the conversion table of the c# types to database types.
The repository is created through specifing the type of the entity the repository should manage.
Afterwards the repository is created an can be used.

The OR Mapper can create the database from the model. This feature can be used by calling the methods.
```csharp
bookRepository.SetupTable();
bookRepository.SetupForeignKeys();
```

For further examples on how to use the OR Mapper, please look at the Unit Tests in the WS2021.SWE.OR-Mapper.Tests project.