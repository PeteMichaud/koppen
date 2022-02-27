# Koppen

This program consumes rainfall and temperature maps to generate [koppen climate 
zones](https://en.wikipedia.org/wiki/K%C3%B6ppen_climate_classification).

## Usage

* After all the following setup is complete, simply run `./Koppen /absolute/path/to/working/directory`
    * When the program completes there will be an `output` directory inside the
      working directory, with a .png image per koppen zone. These can be 
      composited in your favorite image editor. 
* Within the working directory should be a subdirectory named `input`
* Within `input` there should be the following images of whatever type 
(I recommend png):
    * alpha[.png]
        * any area that you want classified should be non-black
        * The `A` channel of the `RGBA` pixel does not matter
    * summerRain[.png]
    * winterRain[.png]
    * summerTemperature[.png]
    * winterTemperature[.png]
    * hottestTemperature[.png] (optional, will fall back to summerTemperature)
    * coldestTemperature[.png] (optional, will fall back to summerTemperature)
 
**Map Formatting Notes:**

* Maps can be any resolution, but the resolution must be identical across all maps
* Map images are opened as 8-bit per channel
    * only the first channel of the pixel is read. This doesn't practically 
      matter unless you want to do something clever with the other channels 
* Rainfall:
    * Black (0)   == 0mm average rainfall per month
    * White (255) == 200mm+ average rainfall per month
* Temperature:
    * The lowest representable temperature is -45C
    * Black (0) == -45C
    * Each degree C corresponds to 1% of the 8-bit value, so the formula is:
        * pixel value from 0-255 / 2.55 == degree C above minimum (-45C)
        * eg. pixel value 128 => 128/2.55 = 50C - 45C => **5C**
    * White (255) == 55C
    * The hottest global average temperatures are normally a couple months after 
      summer solstice 
    * The coldest global average temperatures are normally a couple months after 
      winter solstice 
    * If you only have summer and winter maps, you can use the summer map as the 
      hottest map and the winter map as the coldest map and it'll still classify 
      areas pretty well. 

## Note on the Code Style

I wrote this in a simplified style to make it easy to port to C++. The 
construction I used to make the processing multi-threaded relies on .NET, but 
the logic is intrinsically parallelizable.