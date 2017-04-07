using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace CommercialLite.Droid
{
	[Register("CommercialLite.Droid.Input")]
	public class Input : EditText
	{
		public Input(Context context) : base(context) { }
		public Input(Context context, IAttributeSet attrs) : base(context, attrs) { }
		public Input(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }
		public Input(Context context, IAttributeSet attrs, int defStyle, int defStyleRes) : base(context, attrs, defStyle, defStyleRes) { }
		public Input(IntPtr handle, JniHandleOwnership owner) : base(handle, owner) { }

		public override bool OnKeyPreIme(Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			if (keyCode == Android.Views.Keycode.Back)
				this.ClearFocus();

			return base.OnKeyPreIme(keyCode, e);
		}
	}
}
