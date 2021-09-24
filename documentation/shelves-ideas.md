
# Mouse Smoothing
Just nah, it adds lag and its weird.

It was needed for older PS2 mouses, not on modern 3 million DPI USB mice


In `GamepadProcessor.cs` add a field to store historic mouse positions 

`
        Queue<Vector2i> numbers = new Queue<Vector2i>();
`

Then ammend the FeedMouseCoords method
```
        public Vector2i FeedMouseCoords()
        {
            int MillisecondsPerInput = 1000 / UserSettings.MousePollingRate;
            if (MouseInputTimer.ElapsedMilliseconds >= MillisecondsPerInput)
            {
                Vector2i currentMousePosition = Mouse.GetPosition();
                MouseDirectionPrevious = currentMousePosition - Anchor;

                //recalculate incase they moved the window
                Anchor = MouseAnchor.CalculateAnchor();

                Mouse.SetPosition(Anchor);
                MouseInputTimer.Restart();
                numbers.Enqueue(MouseDirectionPrevious);

                if (numbers.Count > 1)
                {
                    numbers.Dequeue();
                }
                Log.Debug("mouse smoothing numbers {0}", numbers.Count);
                var mouseHistoryEnumerator = numbers.GetEnumerator();
                Vector2f total = new Vector2f(0, 0);
                while (mouseHistoryEnumerator.MoveNext())
                {
                    Log.Debug("mouse smoothing Current X={0}, Y={1}", mouseHistoryEnumerator.Current.X, mouseHistoryEnumerator.Current.Y);

                    total.X += mouseHistoryEnumerator.Current.X;
                    total.Y += mouseHistoryEnumerator.Current.Y;
                }

                Log.Debug("mouse smoothing total X={0}, Y={1}", total.X, total.Y);
                MouseDirectionPrevious.X = (int)(total.X / numbers.Count);
                MouseDirectionPrevious.Y = (int)(total.Y / numbers.Count);
                Log.Debug("mouse smoothing    avg X={0}, Y={1}", total.X, total.Y);
            }
```
