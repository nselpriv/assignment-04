classDiagram 
    Task "*" --> "0..1" User : AssignedTo
    Task "*"--> "*" Tag :Tags
    Task --> State : State
    Tag "*"-->"*" Task :Tasks
    User "1"-->"*" Task : Tasks 

    class State{
    <<enumeration>>
    State : New
    State : Active
    State : Resolved
    State : Closed
    State : Removed
}

    class Task{
        +Title : String
        +AssignedTo : User
        +Description : String
        +State : State
        +Tags : List~Tag~
        +Created : DateTime 
        +StatusUpdated : DateTime

    }
    
    class Tag{
        +Name : string
        +Tasks : List~Task~
    }    
    class User{
        +Name : string
        +Email : string
        +Tasks : List~Task~
    }