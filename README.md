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
- They also see how much expenses there are in a group in total and how much they owe to the group members (or how much they are liable to)

## Bounded Contexts
Based on the requirements above, one can identify operations that deal with either the user, group or expenses. 
Following a general best practice, services were scoped around the business entities and operations performed on them, as they well encapsulate the domain of a given operation set.
Bounded contexts also arised from the grouping above. These go as follows:
- User Management
- Group Management
- Expense Management

## Architecture
The diagram of the project architecture can be found [here](https://drive.google.com/file/d/1CrJ1SDvCy_Mp2TTRiOe08cBiYiAKHQP5/view)

**TODO: Split the diagram into 3 parts, one for each service**

## Docker
Each component is independently Dockerized. Docker Compose is also set up and configured. The project can be started with Docker Compose via issuing the command below, from the root of the repository.
```
docker-compose up
```

## Deployment Strategy

**Gateway**
- **Ports**: Ocelot Gateway is exposed on port 8080, same as the default internal port.
- **Dependencies**: Ocelot Gateway depends on the UserService, GroupService and ExpenseService.

## Communication Protocols

## Authors
- [Sergio Moreno Martínez](https://github.com/SergioMM0)
- [Tawfik Azza](https://github.com/TawfikAzza)
- [Ádám Lőrincz](https://github.com/Ladam0203)
