Zebra Windows Printer Driver 
Build v5.0.0.7

09/05/2003

*******************************
Welcome!

This version of the Zebra Windows Printer driver offers 
many new features and user conveniences. Currently shipping 
Zebra printers using CPCL, EPL2 or ZPL II can now be driven 
using one set of driver files. A common user interface for 
all of the printer languages makes it easy to work with any 
of the supported printers listed below. New utilities, such 
as a Font Downloader, Driver-Cloning and Driver Uninstall 
have been included. The number of printer native bar codes 
and fonts supported by the driver has been increased. The 
driver now also supports printing over USB with Zebra
printers offering USB connectivity. 


*******************************
Installation 

This driver is supported on:
- Windows 95
- Windows 98 SE            
- Windows ME              
- Windows NT v4.0         
- Windows 2000
- Windows XP   

The driver is compatible with:
- Windows 2000 Terminal Services
- Citrix Metaframe v1.8 for Windows 2000.          

(Windows and Windows Terminal Services are registered 
trademarks of Microsoft Corporation. Metaframe is a 
registered trademark of Citrix Systems, Inc.)


How to Install Zebra Printer Drivers  

Drivers can be installed from the Windows "Add Printer Wizard" 
or using the Installation Utility that came with the driver. 
NOTE: you must have sufficient rights on the system being used 
to install the driver. 

To install using the Installation Utility:
1) Uncompress the driver files. Be sure to note the location 
that the files are placed into. 

2) Run the setup.exe utility. 

3) Select the language to install in. Currently, English is the 
only choice.

4) Follow the prompts to select the printer model and port that 
you will be using. 

5) The Utility will install the appropriate files and then run 
the Configuration Wizard to help you configure your new driver. 

6) When the Install/Configure Wizard is complete, your new driver 
is installed and ready for use. 


To install the driver using the Windows "Add Printer Wizard":

1) Uncompress the driver files. Be sure to note the location 
that the files are placed into.

2) Using the START menu select "Settings" and then "Printers". 
The Printers folder will open up. 

3) Double click on "Add Printer" and follow the prompts to begin 
the installation process. 

4) When you come to the dialog screen that lists Manufacturers 
and Printers, select "Have disk".

5) Point to the location that contains the uncompressed driver 
files. Select the zebra.inf file and click "OK" to continue. 

6) Select your printer model from the list of supported printers 
and click the Next button to continue. There are three categories, 
one for CPCL printers, one for EPL printers and one for ZPL 
printers. Click "Next" to continue the installation process. 

7) If your printer is connected via a RS-232 serial connection, 
select "No" to the question "Would you like to print a test page?". 
This is suggested to avoid an error message in case you have not 
configured the port on the computer to match the serial settings 
on the printer. If your printer is connected via a parallel port 
and is ready to print, select "Yes" to print the test page. 

8) Follow the remaining prompts to complete the Wizard. Once the 
install is done your printer driver is now installed and ready 
for use.


*******************************
Supported Printers

The following Zebra printers are supported in this version:

- CPCL printers -
Bravo 2
Bravo 4
Cameo 2
Cameo 2 Plus
Cameo 3
Cameo 3N
Encore 2
Encore 3
Encore 3N
Encore 4
Encore 4SE
QL320
QL420
                
- EPL2 printers - 
Ht-146
LP2443
LP2722
LP2742
LP2824
LP2684
LP2844
TLP2684
TLP2722
TLP2824
TLP2844
TLP2742
TLP3742
2746
2746e
TLP3842

- ZPL II printers (using X.10 or later firmware) - 
DA402
LP2844-Z
PA403
PT403
R-140    
R402
S400
S600
T402
TLP2844-Z
TLP3844-Z
Z4M   
Z6M 
Z4M Plus  
Z6M Plus
90XiIII
90XiIII Plus
96XiIII
96XiIII Plus
105SL
110PAX LH/RH
110XiIII Plus
140XiIII
140XiIII Plus
170XiIII
170XiIII Plus
170PAX LH/RH
220XiIII
220XiIII Plus


The driver files also make provision for the installation
of a ZPL II based "Firmware Loader" driver. The Firmware 
Loader driver is intended to allow a printer to Plug 
and Play with the operating system using a non-model specific 
USB Plug and Play string. This can allow a user to create a 
virtual USB port on the operating system that can then be 
used in downloading printer firmware over a USB connection. 
The Firmware Loader driver is not intended for regular 
use. 


******************************
Tips and Advice

1) If you are upgrading from an earlier version of the 
Zebra printer driver, the font and bar code names have changed. 
It will be necessary to update your document(s) with the new 
names used in this version of the driver to ensure correct 
printing. In some cases, using font or bar codes names from 
prior driver versions can cause graphics to print incorrectly. 
Be sure to update  documents with new font and bar code names. 

2) The ZPL II drivers in this build are intended for use with 
ZPL II printers using firmware X.10 or later. Firmware updates 
are available for selected printers at http://www.zebra.com/

3) Use the Bar Code Passthrough Mode start and end sequence 
with 2D bar codes such as Datamatrix, PDF-417, Micro PDF-417, 
QR Code. The bar code Passthrough Mode can be activated on 
the Options tab of the driver. The default start sequence is 
B{ and the default end sequence is }B. Thus, a line of data to 
be encoded in QR Code would be entered as: B{this is the data}B

4) Use the Bar Code Passthrough Mode start and end sequence 
with bar codes that cover more than one line in your document. 
The bar code Passthrough Mode can be activated on the Options 
tab of the driver. The default start sequence is B{ and the 
default end sequence is }B. Thus, two lines of data to be 
encoded in Code 128 would be entered as: B{thisistwolinesofdata}B

5) Applications will not list the internal fonts unless the 
Zebra printer driver is the selected driver. This can be done 
by setting the driver to be the default driver before starting 
the application or by selecting the Zebra driver while designing 
the document to be printed. 

6) All bar codes are represented using the same display font. 
This means that 2-D bar codes appear as picket fence barcodes 
on the screen, but print as the intended 2-D bar code. The bar 
code display font is not WYSIWYG. Users may have to adjust the 
position of the bar code to achieve the desired placement on 
the printed label. Please keep in mind that if you are printing 
multiple labels with variable data, the dimensions of the bar 
code will change from one label to the next. 

7) Multiple bar codes and fonts of the same type, but with 
different characteristics, can co-exist within the driver. For 
example, it is possible to have more than one Code 128 bar code 
type defined - one with human readable on and one with human 
readable turned off. 

To create a new bar code type, go to the "Printer" tab in the 
driver properties, click on "Bar Code Options" and then on 
"Settings". The "Printer Bar Codes" dialog will display. Click 
on the "New" button, enter a name for the bar code in the "Bar 
Code Properties" prompt and choose the symbology type and 
related bar code characteristics that you require. Click "OK" 
and then "OK" again to confirm your changes. The new bar code 
definition will be displayed in the font list the next time 
you start your application.

To create a new internal font, go to the "Printer" tab in 
the driver properties, click on "Printer Fonts Options" and 
then on "Settings". The "Printer Text Fonts" dialog will display. 
Click on the "New Internal Font" button, enter a name for the 
font in the "Font Properties" prompt and choose the internal 
font and related font characteristics that you require. Click 
"OK" and then "OK" again to confirm your changes. The new bar 
code definition will be displayed in the font list the next 
time you start your application.

8) It is possible to use the Pass Through Mode to include 
additional CPCL, EPL2 or ZPL II commands within the label 
format sent to the printer. For example, this technique can 
be used to send ZPL II commands to RFID printers to encode 
the RFID tag. The commands should be included on the actual 
document and enclosed in the Pass Through Mode start and end 
sequences as defined on the Options tab. The default characters
are ${ to start and }$ to end the pass through. 

9) Using the Cloning Utility requires full Administrative 
rights. This is due to the need to write directly to the 
HKEY_LOCAL_MACHINE registry keys. 

10) The Cloning Utility can only be used to clone drivers of 
the same model number. For example, a LP2844 driver can only 
be cloned to another installation of a LP2844 driver. 

11) The Uninstall Utility may require a restart of the operating 
system to complete the removal of the drivers files. This can 
occur when the operating system or an application has not fully 
released a given driver file. 

12) When a new set of drivers is installed in Windows NT, 2K 
and XP, the operating system creates two files in the WINNT\INF 
directory. The two files are the OEMx.inf and OEMx.pnf files. 
The operating system creates one of each file for each new set 
of a manufacturers drivers. This information is then used to 
present the list of manufactures and printer models in the 
"Add Printer" Wizard. The value of the x changes for each new 
set of drivers introduced into the system, it is possible to 
have OEM1.inf through OEM22.inf (or larger) Uninstalling a 
driver, either by deleting the driver from the Printers folder 
or by using the Uninstall Utility, does not remove these two 
files. If you wish to remove these files you should be very 
cautious that you are removing the OEMx.inf and OEMx.pnf 
files for the Zebra printer driver and not another the ones 
created by another driver. The OEM.inf file created for the 
Zebra driver will have the same contents as the zebra.inf 
file that came with your Zebra printer driver installation files.

13) In certain circumstances lines of rotated text can print on 
top of each other. This can be resolved by setting the driver
to portrait mode and setting the application to landscape mode 
while designing the label.  

******************************
Known Issues

1) The settings on the Printer tab will be reset to the default 
settings if you change a setting on the Document tab. To avoid
this issue, change settings on the Document tab and then make 
changes on the Printer tab. 

2) Some multi-page documents require you to turn on the Smart 
Download feature on the Document tab. If not turned on, some 
pages may print more than once (in place of other pages).

3) The ZPL II Drivers will not print more than 32767 copies of 
a document. This is a Windows limitation. 

4) Some large documents can cause an error in Acrobat Reader 
on Windows 98.

5) The text on the Setup Wizard incorrectly states that the Top 
Adjustment setting alters the amount of label fed out or pulled 
back prior to printing. The dialog should state that the Top 
Adjustment setting moves the entire format up or down from the 
top of the label. For the ZPL II drivers, the Top Adjust setting 
controls the ^LT or Label Top command. The ~TA command is 
controlled through the Vertical Offset control on the Dispense 
Mode dialog. 

6) Graphics print best when a monochrome image is used. Because 
Zebra printers are monochrome printers, color and grayscale 
images will be printed using the dithering method you select on 
the Options tab. There are four available dithering methods, 
each offering some control over the final printed image. 
However, monochrome images will print in a more predictable 
fashion.

7) When using Word, the application forces the driver to insert 
spaces into any edited line of text using the ZPL Font 0 
(ZB:Zebra CG Triumvirate). If you save the document after 
editing the text, but before printing it, the spaces will not 
be inserted on the printed label. 

8) Not all Applications will read internal font information 
from drivers. Some applications will not present the list of 
native fonts and bar codes even if the operating system is 
restarted after installing and the driver is set as the default 
driver. This is a limitation of some applications. 

9) The "Brightness and Contrast" button on the Options dialog 
takes you to a dialog that controls Intensity and Contrast.

10) The HELP text that can be reached from the Dispense Mode 
dialog does not mention the "Use Printer Setting" option.

11) On the ZPL II driver's Font Download dialog, the correct 
?HELP for "Character Set" can be reached if you click on the 
question mark button and then click the question mark cursor 
over the words "Character Set".

12) On the ZPL II driver's Dispense Mode dialog, Pause or Cut 
intervals at or over 1000 are not saved. Additionally, the 
prompt is four characters wide, but a comma is used for values
over 999. 

13) There is no printer image for the Bravo 2 or Bravo 4 on 
the About dialog. 


******************************
Change Log

This is build v5.0.0.7
The driver engine is v1.0.0.26
The language monitor is v1.0.0.26
The model set is v7

v5.0.0.2 - initial release of new platform. 
v5.0.0.3 - added usb printing components to .inf file for 2746e, 
	   TLP-2844Z, LP-2844Z, TLP-2824 AND LP2824
v5.0.0.4 - Thermal Transfer Mode added to PT403, 
	   Updated EPL drivers to include support for 
	   EAN-13, EAN-13 addon 2, EAN-13 addon 5 
v5.0.0.5 - Added 110XiIII Plus 200dpi, 300dpi, 600dpi units
v5.0.0.6 - Added TLP3842.
   	   Added TLP3844-Z.
   	   Added ZMetal 420,430,620,630. 
   	   Added cutter support for T402, LP2844Z and TLP2844Z.
   	   Corrected the displayed print speeds for 2824 & 2844 units.
v5.0.0.7 - ZMetal now Z Plus. 
   	   
******************************
Getting Updates

Zebra Printer Drivers are frequently updated and modified to 
take advantage of new developments. Updated drivers will be 
available on our website:

http://www.zebra.com/



Zebra Technologies Corporation
(C) 2003 ZIH Corp. All rights reserved.
