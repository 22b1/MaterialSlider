# MaterialSlider
'MaterialSlider' is a custom slider control for Windows Forms applications in C#. It provides a sleek, modern look with customizable properties for various use cases.

## Features
 - Orientation: Horizontal or Vertical
 - Customizable Colors: Customize the colors of the bar, elapsed part, and text
 - Rounded Corners: Provides a smooth, modern appearance
 - Mouse Interaction: Mouse hover effects and mouse wheel support
 - Scale Divisions: Configurable major and minor divisions with optional text labels
 - Resizable: Allows setting the size of the slider bar

## Getting Started
### Prerequisites
 - .NET Framework or .NET Core
 - Visual Studio or any compatible C# IDE

### Installation
1. Download the MaterialSlider.cs file from the repository.
2. Add the MaterialSlider.cs file to your Windows Forms project.
3. Build the project to ensure all dependencies are correctly resolved.

### Usage
1. Drag and drop the MaterialSlider control from the toolbox onto your form.
2. Set the properties either through the Properties window in the designer or programmatically.

## Example
Here's a simple example of how to use 'MaterialSlider' in a Windows Forms application:
```csharp
using System;
using System.Windows.Forms;

namespace MaterialSliderExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MaterialSlider slider = new MaterialSlider
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                BarSize = new Size(300, 20),
                BarInnerColor = Color.LightGray,
                ElapsedPenColorTop = Color.LightBlue,
                ElapsedPenColorBottom = Color.Blue,
                Orientation = Orientation.Horizontal,
                Location = new Point(20, 20),
                Size = new Size(300, 50)
            };

            slider.ValueChanged += Slider_ValueChanged;
            Controls.Add(slider);
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            MaterialSlider slider = sender as MaterialSlider;
            MessageBox.Show($"Slider Value: {slider.Value}");
        }
    }
}

```
## Properties
### Appearance
 - BarSize: Set the size of the slider bar 'Size'.
 - BarInnerColor: Set the color of the inner part of the bar 'Color'.
 - ElapsedPenColorTop: Set the top gradient color of the elapsed part 'Color'.
 - ElapsedPenColorBottom: Set the bottom gradient color of the elapsed part 'Color'.

### Behavior
 - Minimum: Set the minimum value of the slider 'decimal'.
 - Maximum: Set the maximum value of the slider 'decimal'.
 - Value: Set or get the current value of the slider 'decimal'.
 - SmallChange: Set the small change amount for the slider 'decimal'.
 - LargeChange: Set the large change amount for the slider 'decimal'.

### Orientation
 - Set the orientation of the slider (Horizontal or Vertical).

### Scale
 - ScaleDivisions: Set the number of main divisions on the slider 'decimal'.
 - ScaleSubDivisions: Set the number of subdivisions between main divisions 'decimal'.
 - ShowSmallScale: Show or hide small subdivisions 'bool'.
 - ShowDivisionsText: Show or hide text labels for divisions 'bool'.

### Events
- ValueChanged: Occurs when the 'Value' property changes.
- Scroll: Occurs when the slider is scrolled.

### Methods
#### CreateRoundedRectanglePath
Creates a rounded rectangle path for smooth corners.
```csharp
private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
```
#### SetProperValue
Ensures the value is within the valid range and updates the control.
```csharp
private void SetProperValue(decimal value)
```
### Customization
To further customize the 'MaterialSlider', you can modify its source code directly. The 'OnPaint' method is a good place to start for custom drawing logic.

# Other
Feel free to contribute to the project by forking the repository and submitting pull requests. If you encounter any issues or have feature requests, please open an issue on GitHub.

Under MIT LICENCE
Made by 22b1 | This is a modified version of [ColorSlider](https://github.com/fabricelacharme/ColorSlider)
