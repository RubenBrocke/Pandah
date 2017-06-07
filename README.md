--Documentation for the Pandah language--

Chapter 1. Hello World

A pandah program always starts itself in the "Main" function of the "Program" class.
to create a running program one should first create this Program class.
this is is done by typing "class Program" in the first line.
like many other languages the class and its name is followed by 2 brackets.
the positioning of these brackets are completely left to the user.
An example:

	class Program
	{

	}

This example uses the backets on a new line.

As said before the next thing a default pandah program has is a main function.
A main function looks like this:

	function Main <- void
	{

	}

The fist thing that should stand out is the use of the "function" keyword.
After the function comes the identifier. This identifier is used to locate the function.
Next comes the assignment operator. 
When used on a function this operator tells the function what parameters its getting.
The "Main" function doesnt get any args by default so we give it void (nothing).
When put together the program should now look like this:

	class Program
	{
		function Main <- void
		{
		
		}
	}
	
Now that we've got the basic setup of a Pandah program we can start the actual programming part.
The easiest way to print a string in Pandah is by using the "print" statement
This is how the print statment works:
	
	print(message);
	
as you can see we can print out "Hello World" by doing

	print("Hello World")

like other languages text is written between double apostrophe's
We can now create our final Hello World program:

	class Program
	{
		function Main <- void
		{
			print("Hello World");
		}
	}

when interpreted you should get the output: Hello World!

Chapter 2: Fundamentals

**Statements**
Everything you do in pandah is started with a statement keyword.
These statement keywords are:
	let
	exec
	init
	print

**Let keyword**
The let keyword is used to change the value of a variable and is used like this:

	let VARIABLE <- VALUE;

VARIABLE is the identifier of the variable you want to change the value of
VALUE is the new value that should be stored in the variable
The value can consist of more than just one value (eg. let var <- 5 + 5; which will result in 10)

**Exec keyword**
The exec keyword is used when one wants to execute a function and is used like this:

	exec FUNCTION();
	
FUNCTION is the indentifier of the function you want to call
Function parameters are to be put inbetween the braces

**Init keyword**
The init keyword is used to create another instance of a class and is used like this:

	init INSTANCE <- CLASS;
	
INSTANCE is the identifier of the instance you want to make.
CLASS is the class of which you want to make an instance.
Instances cannot be made from static class and it will throw an error if you attempt to do so.

**Print keyword**
The print keyword is used to print something to the screen and is used like this:

	print TOPRINT

TOPRINT is the thing you want to print.

TODO: think of more keywords

