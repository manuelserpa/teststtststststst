export default {
    HELP_HEADER: "Help",
    HELP_DETAILS:
`A SECS/GEM message can be build using JSON format, where each SECS/GEM item is represented as an object with the following format:
<pre>{
   "name": "OptionalName",
   "comment": "Optional Comment/Description",
   "type": "A",
   "value": "test"
}</pre>
The supported types are:
A - ASCII (Strings)
BI - Binary Value
BO - Boolean
I1, I2, I4, I8 - Signed Integers
U1, U2, U4, U8 - Unsigned Integers
F4, F8 - Float/Double
L - Lists

Example:
<pre>{
    "type": "L",
    "value": [
        { "type": "A", "value": "test" },
        { "type": "BO", "value": true },
        { "type": "BI", "value": "0x01" },
        { "type": "I1", "value": 1 },
        { "type": "I2", "value": 2 },
        { "type": "I4", "value": 4 },
        { "type": "I8", "value": 8 },
        { "type": "U1", "value": 11 },
        { "type": "U2", "value": 22 },
        { "type": "U4", "value": 44 },
        { "type": "U8", "value": 88 },
        { "type": "F4", "value": 3.14 },
        { "type": "F8", "value": 3.1415 },
        { "type": "L", "value": [
            { "type": "BO", "value": [true, false, false, true] },
            { "type": "BI", "value": [1, 2, 255] },
            { "type": "I1", "value": [1, 2, 3] },
            { "type": "I2", "value": [2, 3, 4] },
            { "type": "I4", "value": [4, 5, 6] },
            { "type": "I8", "value": [8, 9, 10] },
            { "type": "U1", "value": [11, 12, 13] },
            { "type": "U2", "value": [22, 23, 24] },
            { "type": "U4", "value": [44, 45, 46] },
            { "type": "U8", "value": [88, 89, 90] },
            { "type": "F4", "value": [3.14, 3.15, 3.16] },
            { "type": "F8", "value": [3.1415, 3.141516, 3.14151617] }
        ]}
    ]
}</pre>
SECS/GEM item values can be extracted from a message using a path-like string.
The separator is the "/" character.
Indexes start in 1 (1..n). If using a type prefix, will match the order index.

The following examples should help to explain how to use them:
<pre>"/"         -> would return the root node
"/[1]"      -> would return string "test"
"/[4]"      -> Would return integer 1
"/U1"       -> Would return 11
"/U1[1]"    -> Would also return 11
"/L/[3]"    -> Would return [1, 2, 3]
"/[14]/[3]" -> Would also return [1, 2, 3]
</pre>
`,

}
