# character: Jack
Hi, what's your name?
+ [I'm Bob] -> TellName
+ [That's none of your business] -> Rude

== TellName ==
# character: Jack
I'm Jack. Nice to meet you, Bob.
+ [Why did the chicken cross the road?] -> TellJoke
+ [Gotta go now] -> Bye

== Rude ==
# character: Jack
Rude!
-> END

== Bye ==
#character: Jack
Well then, bye!
-> END

== TellJoke ==
# character: Jack
Why?
# character: Bob
...
# character: Jack
...
# character: Bob
...
# character: Jack
Well, bye!
-> END