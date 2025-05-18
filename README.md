# APBD_10
To launch a project insert into appsetting.json provided code:
"ConnectionStrings": {
    "MyDatabse": "put your connection string here"
  }

I divided the solution into three projects to keep it clean and organized.
Each project is responsible for a differrent part of the application:
1. Device.Entities - contains the entities of the database
2. Device.Logic - contains all the logic of the application. It has service to manage and contact with database, dtos to send correct data and validator to validate the input
3. Device.RestApi - contains the minimal api with endpoints to communicate the browser/user with the rest of the application
