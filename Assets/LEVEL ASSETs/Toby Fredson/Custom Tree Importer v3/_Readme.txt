Hi there and welcome to the Custom Tree Importer.

Before starting please make sure that your project is set to use the linear color space in Edit -> Project Settings -> Player.

In case your camera uses deferred rendering you also have to assign the CTI deferred lighting shaders in Edit -> Project Settings -> Graphics:
Under the Built-in shader settings change Deferred to custom, then assign the CTI_Internal-DeferredShading shader.
Also change Deferred Reflections to Custom and assign the CTI_Internal-DeferredReflections shader.

In case you use Unity >= 5.6.0 you will have to enable instancing on each material manually.

Please note: As the project contains shaders which support Unity 5.5 as well as Unity 2017.2 and above you may get a lot of shader warnings – just ignore them :)