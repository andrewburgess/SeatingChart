Seating Chart Genetic Algorithm
===============================

This started off as a genetic algorithm, but took a bit of a turn because now
it focuses more on getting a good result instead of relying on random chance.
I guess now you could consider it more like selective breeding

It means that each generation takes longer to run, but you get ideal results in
fewer iterations. Typical use cases saw ideal results in around 5 or so.

How To Use
==========

Compile the application using your favorite .NET method

There are a few commands to use

<pre>
 help                               : Outputs the commands and some help text
 initialize {input} {population}    : Reads the input configuration file and creates a base population of the specified
                                      size
 parse-names {input} {output}       : Takes a list of names and converts them into the appropriate format for the 
                                      configuration file
 parse-relations {input} {output}   : Takes a list of relations, defined as &lt;left-person&gt; + &lt;right-person&gt; 
                                      and will assign a score of 100 to the relation
 run {generations}                  : Runs the specified number of generations
 save {output}                      : Saves the best arrangment to the specified file, and also saves a more formatted 
                                      version called "pretty-{output}"
</pre>

Input Files
-----------

The input file for the `parse-names` function should look like this:

<pre>
Bob Jenkins
Roger Smith
Namey McNamerson
</pre>

The input file for the `parse-relations` function should look like this:

<pre>
Bob Jenkins + Roger Smith
Roger Smith + Namey McNamerson
</pre>

The input configuration file is a simple JSON object. It follows this spec

<pre>
{
    "NumberOfTables": 2,
    "PeoplePerTable": 4,
    "People": [
        {
            "Name": "Bob Jenkins",
            "Age": 25,
            "Politics": {
                "Left": 5,
                "Right": -10
            },
            "DrunkFactor": 50
        },
        {
            "Name": "Namey McNamerson",
            "Age": 89,
            "Politics": {
                "Left": -100,
                "Right": 100
            },
            "DrunkFactor": 100
        }
    ],
    "Relationships": [
        {
            "Left": "Bob Jenkins",
            "Right": "Namey McNamerson",
            "Score": 40
        }
    ]
}
</pre>

There are a few tweakable settings for the guests to be seated.

Age will have a distance formula applied, `(Left.Age - Right.Age)^2` so that
the further apart in age two people are, the lower the score

Politics contains two parameters, Left and Right, which are used to guage
potential political cohesiveness. These values should range from -100 to 100,
where -100 means they absolute loathing of that political viewpoint, and 100
means undying love for that side.

DrunkFactor can be used to separate raucous drunks from your grandparents
(if need be). The value should range from 0 to infinity. The formula here is basically the negative absolute of the
difference in DrunkFactor. The more distance between the two factors, the more
detrimental the score.

Relationships can be used to define what people can sit with what other people.
Specify the names of each person and the score to use for their relationship 
(typically between -100 and 100) where -100 means they shouldn't be anywhere
near each other, and 100 means they are inseparable.