FilterTest
==========

Using Math.NET signal processing library [neodym](https://github.com/mathnet/mathnet-neodym.git) for filter design and online filtering

Include
```
MathNet.Iridium.dll
MathNet.Neodym.dll
nunit.framework.dll
```
in the project.

If you come across this error:
```
Strong name validation failed. (Exception from HRESULT: 0x8013141A)
```
Edit windows register:
```
[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\StrongName\Verification\*,c061a3ec32cc0c6f]
```
where `c061a3ec32cc0c6f` is Neodym.dll's PublicKeyToken, see [this](http://goo.gl/z6yurS) for details.
