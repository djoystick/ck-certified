rem  ------------------------ LocBaml to output ------------------------
copy ..\..\..\..\Setup\LocBaml.exe ..\..\..\..\Output\Debug\LocBaml.exe

rem  ------------------------ Plugin contents ------------------------
copy ..\..\..\..\Output\Debug\Plugins\SimpleSkin.dll ..\..\..\..\Output\Debug\SimpleSkin.dll
mkdir ..\..\..\..\Output\Debug\en-US
copy ..\..\..\..\Output\Debug\Plugins\en-US\SimpleSkin.resources.dll ..\..\..\..\Output\Debug\en-US\SimpleSkin.resources.dll

rem  ------------------------ generate with LocBaml ------------------------
cd ..\..\..\..\Output\Debug\
mkdir Plugins\fr-FR
LocBaml /generate en-US\SimpleSkin.resources.dll /trans:..\..\Plugins\Accessibility\SimpleSkin\Locale\fr-FR.txt /cult:fr-FR /out:Plugins\fr-FR

rem  ------------------------ clean ------------------------
del en-US\SimpleSkin.resources.dll
del LocBaml.exe
del SimpleSkin.dll

pause