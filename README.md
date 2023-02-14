This is the basis for a videogame that's been sitting in my head, in various forms, for about five years. Whether or not it's actually fun is is a matter of public debate, or it will be when this project becomes relevant enough for people to care about. 
The core mechanic is likely familiar to people who have played certain tabletop RPGs: take an integer attribute modifier as input, perform some operation that many times, and return the number of successful operations as output. I decided to play with that a little bit more, however. 
We'll use the humble d8 as our basic example. Rolling a d8, as I'm sure you know, generates a number between one and eight. We'll set our success value as six, succeeding 37.5% of the time. Thus, when we roll five d8s, we'll get (.375*5) an average of 1.875 successes. Some of you may be bored as this is basic expectation, some may be bored as stats aren't your thing.
There are two modifications to this system that I'd hoped would make it more interesting, at least to me. 
The first: rolling the maximum value on a die yields not only a success, but allows us to make two more rolls. This means that an input of five can - at least theoretically - yield a nigh infinite output. It's not as impressive as it sounds.
Second, a resource can be spent to increase or decrease the size of the die, leaving the success value unchanged. 
As you can probably guess, increasing the size of the die leads to a higher expected value, but fewer incidents of truly remarkable output.
Conversely, decreasing the size of the die decreases the expected value, but makes explosively high outcomes comparatively *more* likely.

More interestingly, I realized that it's very easy to take those same input parameters (attribute, randomness, manipulation), and apply them to different flavors of randomizer. Thus the option of playing with a traditional deck of playing cards works similarly. As the code only cares about the output of the function, there are no issues of compatibility between card and dice using entities. 

All of these mechanics are little more than mathematical curiousities if not implemented in an interesting game.

I'm working on it.
