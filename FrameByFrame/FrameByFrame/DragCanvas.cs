using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace FrameByFrame
{
    public class DragCanvas : Canvas
    {
        #region Data

        // Stores a reference to the UIElement currently being dragged by the user.
        private UIElement elementBeingDragged;

        private UIElement elementBeingResized;

        // Keeps track of where the mouse cursor was when a drag operation began.       
        private Point origCursorLocation;

        // The offsets from the DragCanvas' edges when the drag operation began.
        private double origHorizOffset, origVertOffset;

        // Keeps track of which horizontal and vertical offset should be modified for the drag element.
        private bool modifyLeftOffset, modifyTopOffset;

        // True if a drag operation is underway, else false.
        private bool isDragInProgress;

        // True if a drag operation is underway and the mouse has moved since the process has started. This is used
        // in order to determine on left mouse up whether we should display the resize adorners or not.
        private bool hasMouseMovedInDragInProgress;

        private AdornerLayer adornerLayer;

        #endregion // Data

        #region Attached Properties

        #region CanBeDragged

        public static readonly DependencyProperty CanBeDraggedProperty;
        public static readonly DependencyProperty LineBelongsToBaseGridProperty;

        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
                return false;

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }

        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
                uiElement.SetValue(CanBeDraggedProperty, value);
        }

        #endregion // CanBeDragged

        #endregion // Attached Properties

        #region Dependency Properties

        public static readonly DependencyProperty AllowDraggingProperty;
        public static readonly DependencyProperty AllowDragOutOfViewProperty;

        #endregion // Dependency Properties

        #region Static Constructor

        static DragCanvas()
        {
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true));

            AllowDragOutOfViewProperty = DependencyProperty.Register(
                "AllowDragOutOfView",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(false));

            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(true));

            LineBelongsToBaseGridProperty = DependencyProperty.RegisterAttached(
                "LineBelongsToBaseGrid",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(false));
        }

        #endregion // Static Constructor

        #region Constructor

        /// <summary>
        /// Initializes a new instance of DragCanvas.  UIElements in
        /// the DragCanvas will immediately be draggable by the user.
        /// </summary>
        public DragCanvas()
        {
        }

        #endregion // Constructor

        #region Interface

        #region AllowDragging

        /// <summary>
        /// Gets/sets whether elements in the DragCanvas should be draggable by the user.
        /// The default value is true.  This is a dependency property.
        /// </summary>
        public bool AllowDragging
        {
            get { return (bool)base.GetValue(AllowDraggingProperty); }
            set { base.SetValue(AllowDraggingProperty, value); }
        }

        #endregion // AllowDragging

        #region AllowDragOutOfView

        /// <summary>
        /// Gets/sets whether the user should be able to drag elements in the DragCanvas out of
        /// the viewable area.  The default value is false.  This is a dependency property.
        /// </summary>
        public bool AllowDragOutOfView
        {
            get { return (bool)GetValue(AllowDragOutOfViewProperty); }
            set { SetValue(AllowDragOutOfViewProperty, value); }
        }

        #endregion // AllowDragOutOfView

        #region BringToFront / SendToBack

        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is in front of every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// decremented by one.
        /// </summary>
        /// <param name="targetElement">
        /// The element to be sent to the front of the z-order.
        /// </param>
        public void BringToFront(UIElement element)
        {
            this.UpdateZOrder(element, true);
        }

        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is behind every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// incremented by one.
        /// </summary>
        /// <param name="targetElement">
        /// The element to be sent to the back of the z-order.
        /// </param>
        public void SendToBack(UIElement element)
        {
            this.UpdateZOrder(element, false);
        }

        #endregion // BringToFront / SendToBack

        #region ElementBeingDragged

        /// <summary>
        /// Returns the UIElement currently being dragged, or null.
        /// </summary>
        /// <remarks>
        /// Note to inheritors: This property exposes a protected 
        /// setter which should be used to modify the drag element.
        /// </remarks>
        public UIElement ElementBeingDragged
        {
            get
            {
                if (!this.AllowDragging)
                    return null;
                else
                    return this.elementBeingDragged;
            }
            protected set
            {
                if (this.elementBeingDragged != null)
                    this.elementBeingDragged.ReleaseMouseCapture();

                if (!this.AllowDragging)
                    this.elementBeingDragged = null;
                else
                {
                    if (DragCanvas.GetCanBeDragged(value))
                    {
                        this.elementBeingDragged = value;
                        this.elementBeingDragged.CaptureMouse();
                    }
                    else
                        this.elementBeingDragged = null;
                }
            }
        }

        #endregion // ElementBeingDragged

        #region FindCanvasChild

        /// <summary>
        /// Walks up the visual tree starting with the specified DependencyObject, 
        /// looking for a UIElement which is a child of the Canvas.  If a suitable 
        /// element is not found, null is returned.  If the 'depObj' object is a 
        /// UIElement in the Canvas's Children collection, it will be returned.
        /// </summary>
        /// <param name="depObj">
        /// A DependencyObject from which the search begins.
        /// </param>
        public UIElement FindCanvasChild(DependencyObject depObj)
        {
            while (depObj != null)
            {
                // If the current object is a UIElement which is a child of the
                // Canvas, exit the loop and return it.
                UIElement elem = depObj as UIElement;
                if (elem != null && base.Children.Contains(elem))
                    break;

                // VisualTreeHelper works with objects of type Visual or Visual3D.
                // If the current object is not derived from Visual or Visual3D,
                // then use the LogicalTreeHelper to find the parent element.
                if (depObj is Visual || depObj is Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }
            return depObj as UIElement;
        }

        #endregion // FindCanvasChild

        #endregion // Interface

        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            return;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            //int b;
            //b = 10;
            return;
        }

        #region OnPreviewMouseLeftButtonDown

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            this.isDragInProgress = false;

            // If we have a mouse button down, check whether elementBeingResized is not null.
            // If it's not, it means that we were resizing an element and we have now clicked somewhere else,
            // so remove its resizing adorner.
            if ((this.elementBeingResized != null) && (this.adornerLayer != null))
            {
                var adorners = adornerLayer.GetAdorners(elementBeingResized);
                if (adorners != null)
                    adornerLayer.Remove(adorners[0]);
            }
            // Cache the mouse cursor location.
            this.origCursorLocation = e.GetPosition(this);

            // Walk up the visual tree from the element that was clicked, 
            // looking for an element that is a direct child of the Canvas.
            this.elementBeingResized = this.ElementBeingDragged = this.FindCanvasChild(e.Source as DependencyObject);
            if (this.ElementBeingDragged == null)
                return;

            if ((bool)ElementBeingDragged.GetValue(LineBelongsToBaseGridProperty))
            {
                this.ElementBeingDragged = null;
                return;
            }

            // Get the element's offsets from the four sides of the Canvas.
            double left = Canvas.GetLeft(this.ElementBeingDragged);
            double right = Canvas.GetRight(this.ElementBeingDragged);
            double top = Canvas.GetTop(this.ElementBeingDragged);
            double bottom = Canvas.GetBottom(this.ElementBeingDragged);

            // Calculate the offset deltas and determine for which sides
            // of the Canvas to adjust the offsets.
            this.origHorizOffset = ResolveOffset(left, right, out this.modifyLeftOffset);
            this.origVertOffset = ResolveOffset(top, bottom, out this.modifyTopOffset);

            // Set the Handled flag so that a control being dragged 
            // does not react to the mouse input.
            e.Handled = true;

            this.isDragInProgress = true;
        }

        #endregion // OnPreviewMouseLeftButtonDown

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
        }

        #endregion

        #region OnPreviewMouseMove

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            // If no element is being dragged, there is nothing to do.
            if (this.ElementBeingDragged == null || !this.isDragInProgress)
                return;

            hasMouseMovedInDragInProgress = true;

            // Get the position of the mouse cursor, relative to the Canvas.
            Point cursorLocation = e.GetPosition(this);

            // These values will store the new offsets of the drag element.
            double newHorizontalOffset, newVerticalOffset;

            #region Calculate Offsets

            // Determine the horizontal offset.
            if (this.modifyLeftOffset)
                newHorizontalOffset = this.origHorizOffset + (cursorLocation.X - this.origCursorLocation.X);
            else
                newHorizontalOffset = this.origHorizOffset - (cursorLocation.X - this.origCursorLocation.X);

            // Determine the vertical offset.
            if (this.modifyTopOffset)
                newVerticalOffset = this.origVertOffset + (cursorLocation.Y - this.origCursorLocation.Y);
            else
                newVerticalOffset = this.origVertOffset - (cursorLocation.Y - this.origCursorLocation.Y);

            #endregion // Calculate Offsets

            if (!this.AllowDragOutOfView)
            {
                #region Verify Drag Element Location

                // Get the bounding rect of the drag element.
                Rect elemRect = this.CalculateDragElementRect(newHorizontalOffset, newVerticalOffset);

                //
                // If the element is being dragged out of the viewable area, 
                // determine the ideal rect location, so that the element is 
                // within the edge(s) of the canvas.
                //
                bool leftAlign = elemRect.Left < 0;
                bool rightAlign = elemRect.Right > this.ActualWidth;

                if (leftAlign)
                    newHorizontalOffset = modifyLeftOffset ? 0 : this.ActualWidth - elemRect.Width;
                else if (rightAlign)
                    newHorizontalOffset = modifyLeftOffset ? this.ActualWidth - elemRect.Width : 0;

                bool topAlign = elemRect.Top < 0;
                bool bottomAlign = elemRect.Bottom > this.ActualHeight;

                if (topAlign)
                    newVerticalOffset = modifyTopOffset ? 0 : this.ActualHeight - elemRect.Height;
                else if (bottomAlign)
                    newVerticalOffset = modifyTopOffset ? this.ActualHeight - elemRect.Height : 0;

                #endregion // Verify Drag Element Location
            }

            #region Move Drag Element

            if (this.modifyLeftOffset)
                Canvas.SetLeft(this.ElementBeingDragged, newHorizontalOffset);
            else
                Canvas.SetRight(this.ElementBeingDragged, newHorizontalOffset);

            if (this.modifyTopOffset)
                Canvas.SetTop(this.ElementBeingDragged, newVerticalOffset);
            else
                Canvas.SetBottom(this.ElementBeingDragged, newVerticalOffset);

            #endregion // Move Drag Element
        }

        #endregion // OnPreviewMouseMove

        #region OnHostPreviewMouseUp

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if ((elementBeingResized != null) && !hasMouseMovedInDragInProgress)
            {
                // If no call to MouseMove has been issues during the drag process, it means that the user wants to resize it.
                adornerLayer = AdornerLayer.GetAdornerLayer(elementBeingResized);
                adornerLayer.Add(new ResizingAdorner(elementBeingResized));
            }
            hasMouseMovedInDragInProgress = false;

            // Reset the field whether the left or right mouse button was 
            // released, in case a context menu was opened on the drag element.
            this.ElementBeingDragged = null;
        }

        #endregion // OnHostPreviewMouseUp

        #region HostEventHandlers

        #endregion // Host Event Handlers

        #region Private Helpers

        #region CalculateDragElementRect

        /// <summary>
        /// Returns a Rect which describes the bounds of the element being dragged.
        /// </summary>
        private Rect CalculateDragElementRect(double newHorizOffset, double newVertOffset)
        {
            if (this.ElementBeingDragged == null)
                throw new InvalidOperationException("ElementBeingDragged is null.");

            Size elemSize = this.ElementBeingDragged.RenderSize;

            double x, y;

            if (this.modifyLeftOffset)
                x = newHorizOffset;
            else
                x = this.ActualWidth - newHorizOffset - elemSize.Width;

            if (this.modifyTopOffset)
                y = newVertOffset;
            else
                y = this.ActualHeight - newVertOffset - elemSize.Height;

            Point elemLoc = new Point(x, y);

            return new Rect(elemLoc, elemSize);
        }

        #endregion // CalculateDragElementRect

        #region ResolveOffset

        /// <summary>
        /// Determines one component of a UIElement's location 
        /// within a Canvas (either the horizontal or vertical offset).
        /// </summary>
        /// <param name="side1">
        /// The value of an offset relative to a default side of the 
        /// Canvas (i.e. top or left).
        /// </param>
        /// <param name="side2">
        /// The value of the offset relative to the other side of the 
        /// Canvas (i.e. bottom or right).
        /// </param>
        /// <param name="useSide1">
        /// Will be set to true if the returned value should be used 
        /// for the offset from the side represented by the 'side1' 
        /// parameter.  Otherwise, it will be set to false.
        /// </param>
        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            // If the Canvas.Left and Canvas.Right attached properties 
            // are specified for an element, the 'Left' value is honored.
            // The 'Top' value is honored if both Canvas.Top and 
            // Canvas.Bottom are set on the same element.  If one 
            // of those attached properties is not set on an element, 
            // the default value is Double.NaN.
            useSide1 = true;
            double result;
            if (Double.IsNaN(side1))
            {
                if (Double.IsNaN(side2))
                {
                    // Both sides have no value, so set the
                    // first side to a value of zero.
                    result = 0;
                }
                else
                {
                    result = side2;
                    useSide1 = false;
                }
            }
            else
            {
                result = side1;
            }
            return result;
        }

        #endregion // ResolveOffset

        #region UpdateZOrder

        /// <summary>
        /// Helper method used by the BringToFront and SendToBack methods.
        /// </summary>
        /// <param name="element">
        /// The element to bring to the front or send to the back.
        /// </param>
        /// <param name="bringToFront">
        /// Pass true if calling from BringToFront, else false.
        /// </param>
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            if (!base.Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            #endregion // Safety Check

            #region Calculate Z-Indici And Offset

            // Determine the Z-Index for the target UIElement.
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in base.Children)
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
            }
            else
            {
                elementNewZIndex = 0;
            }

            // Determine if the other UIElements' Z-Index 
            // should be raised or lowered by one. 
            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(element);

            #endregion // Calculate Z-Indici And Offset

            #region Update Z-Indici

            // Update the Z-Index of every UIElement in the Canvas.
            foreach (UIElement childElement in base.Children)
            {
                if (childElement == element)
                    Canvas.SetZIndex(element, elementNewZIndex);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);

                    // Only modify the z-index of an element if it is  
                    // in between the target element's old and new z-index.
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion // Update Z-Indici
        }

        #endregion // UpdateZOrder

        #endregion // Private Helpers
    }

    public class ResizingAdorner : Adorner
    {
        Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;

        // Initialize the ResizingAdorner.
        public ResizingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = VisualWorker.GetTop(adornedElement);
            //double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            //Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
            VisualWorker.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = VisualWorker.GetLeft(adornedElement);
            //double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            VisualWorker.SetLeft(adornedElement, left_old - (width_new - width_old));

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = VisualWorker.GetTop(adornedElement);
            //double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            //Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
            VisualWorker.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = VisualWorker.GetLeft(adornedElement);
            //double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            //Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
            VisualWorker.SetLeft(adornedElement, left_old - (width_new - width_old));
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 1;
           // cornerThumb.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Thumb 1.jpg")));
            visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }

    public class VisualWorker
    {
        public static void SetTop(UIElement uie, double top)
        {
            var frame = uie as FrameworkElement;
            frame.Margin = new Thickness(frame.Margin.Left, top, frame.Margin.Right, frame.Margin.Bottom);
        }
        public static void SetLeft(UIElement uie, double left)
        {
            var frame = uie as FrameworkElement;
            frame.Margin = new Thickness(left, frame.Margin.Top, frame.Margin.Right, frame.Margin.Bottom);
        }
        public static double GetTop(UIElement uie)
        {
            return (uie as FrameworkElement).Margin.Top;
        }
        public static double GetLeft(UIElement uie)
        {
            return (uie as FrameworkElement).Margin.Left;
        }
    }
}
