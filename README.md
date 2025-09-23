# RPG-Mote
A basic library that can be used for the base structures that are common in all RPGs. Future plans is to create dedicated examples where these classes will be used for the base gameplay. The intention is for this to be as portable as possible, available for any platform or application interface.

## How Will This be Used
There is a data folder containing json files to help template the basic classes and structures that are common across almost all action RPGs.

These files in conjunction with a home-brewed generator, we can generate these class files for a multitude of languages, where these class files will be available for your project. 

## Future Considerations
I plan on using a base JSon parser to get the objects within the generator code, from there an API will be made available to generating the file for your given language. 

Due to every language being unique, this API will be made available for users to create plugins that can load in their specific language without rewriting the application. I will write the generators for my favorite and most common languages I use regularly: C++, C#, Python, and Java.

## Why Data and Generate Rather Than Just One Language Structure
I originally wanted to write the library as a struct that can be transferred via DLL loading into any base language, that way I could write this once and leave it up to the user. This left a multitude of issues:

1. It will be up to the user to know how to load dlls into their application, which is most often not intuitive (especially for the Windows APIs).
2. This way can make the mileage vary significantly. The user would now be relying on any given API to be readable _and_ usable, plus would require a lot of trust from users on my end that the structures contain what they want.
3. On that last point, again, every language is different, and each have a different way to make an RPG. Not having native access to the structures leads to lack of extensibility, instead of creating templates I would be creating guard rails for the user to work in. I want to help users start up with basic classes, not force them to use my structures as written. 

For my vision, if I want to be able to use these templates for my projects - whether that project is home-brewed, Unity, Unreal Engine, Godot, or even Pygame - I would have to be able to have these structures native for their parent languages. Future game development might not even use these engines, but instead something else such as a Rust-based game engine, and I would liek this to still apply.