# Pharo Virtual Reality IDE (PharoVRIDE)
A Virtual Reality Interactive Development Environment made in Unity that supports Pharo programming language.

<i><b>Note:</b> This is still an early WIP, so there could be lot of features missing</i>

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(1).png)

## What is PharoVRIDE?

PharoVRIDE is an attempt to make an entire IDE inside a VR game. The main goal of this project is having a big and personalized space, where the programmer can not only develop software in Pharo, but also see and interact with 3D representations of the IDE and Objects in real time.

Our main goals for this project are:
<ul>
  <li>Having one or multiple instances of interactable widgets, like a text editor, system browser, inspector, and debugger.</li>
  <li>Being able to save your environment and continue from where you left.</li>
  <li>Being able to write and see the execution of your project on the go, both in the same shared space.</li>
  <li>Having 3D visualization of your code, such as an object's size, performance, etc.</li>
  <li>Connecting with other programmers on the same space</li>
</ul>

It works much like the Pharo IDE: create and search for classes using the System Browser, instantiate objects using the Playground, and inspect their variables using the inspector.

### The System Browser

Just like Pharo's System Browser, you can use the text editor to create your own classes. In order to do so, write your piece of code and execute the corresponding shorcut. Classes are shown in the left window, while their methods are shown in the right window. Once you select one of the objects, you can see its source code.

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(2).png)
![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(3).png)

### The Playground
Once you've defined your own objects, or if you just want to play with some basic operations, write some code using the playground's text editor. Depending on the shorcut you use, you can either execute your code or print the result after selecting a statement.

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(4).png)

## How does it work?

### Executing Pharo code
The software uses a C# script that makes HTTP request to a Pharo server. The server is initialized using the Zinc HTTP package. Take this line as an example:

```
ZnReadEvalPrintDelegate startInServerOn: 1701.
```

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22.png)

A Zinc instruction looks something like this:

```
ZnClient new
    url: 'http://localhost:1701/repl';
    contents: '42 factorial';
    post.
```

Since it is a POST request that has to be done from a C# code, we use HTTP client. Then, the instruction looks like this:

```
using System.Net.Http;
...

public class PharoRequests
{
    private static readonly HttpClient client = new HttpClient();

    async void PharoCall()
    {   
        var content = new StringContent("42 factorial", Encoding.UTF8);

        var response = await client.PostAsync("http://localhost:1701/repl", content);

        var responseString = await response.Content.ReadAsStringAsync();
    }
}
```

Pharo will warn you that there may be linebreaks inside your Pharo code, but it will work anyways (at least on my Windows machine).

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(7).png)

Once a new class or method is created, a new Unity GameObject is created and placed in its corresponding scrollable window.

### Working with prefabs

Each element is instantiated as a prefab, and most of the prefabs are Canvas. The most relevant prefabs are the following:

<ul>
  <li>Scrollable Window: it is adapted to fit its width and height to its content. It can be used as a class container and a method container</li>
  <li>Text Editor: requires Text Mesh Pro, which uses Rich Text in order to produce Text Highlighting</li>
</ul>

![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(6).png)
![alt text](https://github.com/Vito217/PharoVRIDE/blob/master/Screenshots/2020-04-22-(5).png)

### Text Highlighting

The text highlightning is based on regular expressions. You can specify a list of keywords to be used, and define regular expressions that matches those keywords.

## What is currently working:

<ul>
  <li>Browser Text Editor and Scrollable Windows: define classes or methods, and select them to see their sourcecode.</li>
  <li>Playground: writes code and prints its result.</li>
  <li>Inspector: inspect a variable and its value.</li>
</ul>

## What is NOT working (a.k.a. TODO)

<ul>
  <li>Any kind of logging. It won't display any error, warning or std prints.</li>
  <li>Error/Crash handlers.</li>
  <li>VR camera and controls.</li>
  <li>Graphical visualizations of the executed code.</li>
  <li>Calling stored classes (they won't reappear when the software is closed)</li>
  <li>A list of bugs (shown below).</li>
</ul>

## List of Bugs

<ul>
  <li>The Pharo server must have a default author. Otherwise, if the user tries to define a method, the server will be waiting for an author. If the user selects the yet undefined method, the software will crash.</li>
  <li>Some details in the text editor, such as incorrect highlightning, coloring, blank spaces, etc.</li>
</ul>

## Controls

<ul>
  <li>Left Click: Select and Drag</li>
  <li>Arrows: Move Player</li>
  <li>Mouse movement: Move Camera</li>
  <li>Ctrl + G: Execute (and print, only if you are in Playground)</li>
  <li>Ctrl + H: Inspect the selected variable (only in Playground)</li>
  <li>Ctrl + Q: Invoke a new Browser</li>
  <li>Ctrl + E: Invoke a new Playground</li>
</ul>

## Channel Log

<a href="https://github.com/Vito217/PharoVRIDE/blob/master/CHANGELOG.md">Click here to see the Channel Log</a>

## Download

<a href="https://mega.nz/file/VIYQTChT#-UR2-MGmffdcovX4cm85d-vqzqKn5yANCj1o3KziUAs">VRIDE v0.5.0 for Windows and Mac OS</a>
