# EzMoney
**SYS Synopsis**<br/>
Broad Approach<br/>
EASV - 2024

## Introduction
Managing shared expenses have been a hassle ever since people were living in caves, thousands of years ago.
Nowadays, the same issue is still present between groups of friends/family.

EzMoney application which will allow people to share the  load on everyday expenses, for example grocery shopping,  restaurant, bar, trip etc. People can create groups in which they can record expenses that are shared among selected members. Members can log and keep track of who funded each payment and how much they owe to each other.

The application is expected to go viral, with many users, so preparing a robust system is essential. Though the project only covers an MVP of the application it is to be done in a fashion so that it can scale easily - thus, practices and technologies that are recommended for distributed systems are to be taken into consideration.

## Scope/Features
- Users of the application can register via their phone number and their name
- Users may log in with their phone number
- The user can see a list of their groups if they part of any, create a group or join a group via a group token.
- In a group, the user sees all the expenses added by the group members and can also add their own, specifying whom the expense is shared with within the group.
- See how much expenses there are in a group in total

The problem statement implies one more feature, which is
- Group member should be able to see how much they owe to the group members (or how much they are liable to)

Which is not covered in the MVP.

## Bounded Contexts
Based on the requirements above, one can identify operations that deal with either the user, group or expenses. 
Following a general best practice, services were scoped around the business entities and operations performed on them, as they well encapsulate the domain of a given operation set.
Bounded contexts also arised from the grouping above. These go as follows:
- User Management
- Group Management
- Expense Management

## Architecture
The following diagrams illustracte the processes linked to the bounded context,
joined with the services that are responsible for the operations.
- [User](https://drive.google.com/file/d/1biWHwEevp3qnbZef6QwccGr6lgAbbyHB/view)
- [Group](https://drive.google.com/file/d/1hN6HFOzoI3Di3ZPKWjY4TTGU0u-MzSKw/view)
- [Expense](https://drive.google.com/file/d/17u6HkDyi6n1s4ICh9JWZfozaOjOGmZ73/view)

## Docker
Each component is independently Dockerized. Docker Compose is also set up and configured. The project can be started with Docker Compose via issuing the command below, from the root of the repository.
```
docker-compose up
```

## Deployment Strategy

**Gateway**
- **Ports**: Ocelot Gateway is exposed on port 8080, same as the default internal port.
- **Dependencies**: Ocelot Gateway depends on the UserService, GroupService, AuthService and ExpenseService.

**UserService**
- **Ports**: UserService's exposed port is 1000, it's internal port is 8080.
- **Dependencies**: UserService depends on the UserRepository and RabbitMQ.

**AuthService**
- **Ports**: AuthService's exposed port is 1500, it's internal port is 8080.
- **Dependencies**: AuthService depends on the UserService. (For checking if the user exists and off-loading user creation)

**GroupService**
- **Ports**: GroupService's exposed port is 2000, it's internal port is 8080.
- **Dependencies**: GroupService depends on the GroupRepository and RabbitMQ.

**ExpenseService**
- **Ports**: ExpenseService's exposed port is 3000, it's internal port is 8080.
- **Dependencies**: ExpenseService depends on the ExpenseRepository and RabbitMQ.

**UserRepository**
- **Ports**: UserRepository is exposed on port 4000, it's internal port is 8080.
- **Dependencies**: UserRepository depends on RabbitMQ and uses SQLite for storage.

**GroupRepository**
- **Ports**: GroupRepository is exposed on port 5000, it's internal port is 8080.
- **Dependencies**: GroupRepository depends on RabbitMQ and uses SQLite for storage.

**ExpenseRepository**
- **Ports**: ExpenseRepository is exposed on port 6000, it's internal port is 8080.
- **Dependencies**: ExpenseRepository depends on RabbitMQ and uses SQLite for storage.

**RabbitMQ**
- **Ports**: RabbitMQ is exposed on port 5672, same as the default internal port. The management interface is exposed on port 15672, same as the default internal port.

**Environment Variables**
The environment variables are set in the docker-compose.yml file. The environment variables are set for the services to communicate with each other. These may be changed to suit the needs of the deployment environment, but may be done with caution, so that the services can communicate with each other.

## Communication Protocols
The services communicate with each other via **HTTP**.

The Gateway service is responsible for **routing the requests** to the appropriate service.

The services communicate with the repositories via **messaging** (RabbitMQ).


## Authors
- [Sergio Moreno Martínez](https://github.com/SergioMM0)
- [Ádám Lőrincz](https://github.com/Ladam0203)
- [Tawfik Azza](https://github.com/TawfikAzza)
