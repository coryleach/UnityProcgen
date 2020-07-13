<p align="center">
<img align="center" src="https://raw.githubusercontent.com/coryleach/UnityPackages/master/Documentation/GameframeFace.gif" />
</p>
<h1 align="center">Gameframe.Procgen 👋</h1>

<!-- BADGE-START -->
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/coryleach/UnityProcgen?include_prereleases)
[![license](https://img.shields.io/github/license/coryleach/UnityProcgen)](https://github.com/coryleach/UnityProcgen/blob/master/LICENSE)

[![twitter](https://img.shields.io/twitter/follow/coryleach.svg?style=social)](https://twitter.com/coryleach)
<!-- BADGE-END -->

Library of utilitities for procedural generation

## Quick Package Install

#### Using UnityPackageManager (for Unity 2019.3 or later)
Open the package manager window (menu: Window > Package Manager)<br/>
Select "Add package from git URL...", fill in the pop-up with the following link:<br/>
https://github.com/coryleach/UnityProcgen.git#0.0.6<br/>

#### Using UnityPackageManager (for Unity 2019.1 or later)

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.gameframe.procgen": "https://github.com/coryleach/UnityProcgen.git#0.0.6",
    ...
  },
}
```

<!-- DOC-START -->
## Sample Output

<img src="https://github.com/coryleach/UnityProcgen/blob/master/Images/Sample_01.PNG?raw=true" />

<img src="https://github.com/coryleach/UnityProcgen/blob/master/Images/Sample_00.PNG?raw=true" />

## Usage

This code is still in a pretty rough state but I've uploaded it here for those interested.  
There is a demo package included in the demo folder. After importing this as a package just double click it to import.  
  
The demo requires that you import Shader Graph and the Universal Render Pipeline packages from the package manager.  
Ensure both 'depth texture' and 'opaque texture' options are enabled on your renderer asset or set unity to use the included render assets.
The demo may require 2019.3
<!-- DOC-END -->

## Author

👤 **Cory Leach**

* Twitter: [@coryleach](https://twitter.com/coryleach)
* Github: [@coryleach](https://github.com/coryleach)


## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [Gameframe.Packages](https://github.com/coryleach/unitypackages)_
