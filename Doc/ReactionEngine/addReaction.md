

## Create and add new reactions type


The reaction engine was made so as to add and remove reactions easily.
In order to create and add a new reaction, follow theses steps:


### 1. Create a New class


You have to make a new class that inherit from the existing IReaction
interface and implement the following method :


	    public abstract void react(ArrayList molecules);

This function will be call at each turn (time) and should implement the calculus
of the reaction. You also have to modify the concentration of molecules given in
parameter.

You have to add new lines in the IReaction.copyReaction function.
Example :


	    if (r as NewReactionType != null)
	       return new NewReactionType(r as NewReactionType);


You have to implement a copy constructor in your new reaction class.

*You can copy the Degradation reaction in example if needed*


### 2. Adding it to a Medium

Each reaction is contained in a medium and should be assigned manualy.
There is a notion of sets. A set is simply a list of reactions.
Each medium have a set, so a list of reactions.

You only have to create a class wich will load your reactions from a file for example.
Then you have to call the loading function of your class in the FileLoader.loadReactions
in FileLoader.cs. This function should respect this prototype :

   		  bool loading(XmlNode node, LinkedList<IReaction> reactions)

And you have to add at the end of the reactions param the new instance of your reaction.

*See AllosteryFileLoader.cs if you want an example*