using System;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using System.Collections.Generic;
using Android.Content;

namespace Euro2016Wallpapers
{
	public class TypeFaceListAdapter:BaseAdapter<TypefaceModel>
	{
		private Context _context;
		private List<TypefaceModel> _typefaces;

		public TypeFaceListAdapter (Context context, List<TypefaceModel> typefaces)
		{
			_context = context;
			_typefaces = typefaces;

		}

		public override int Count {
			get { return _typefaces.Count; }
		}

		public override TypefaceModel this [int position] {
			get { return _typefaces [position]; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = _typefaces [position];
			var view = convertView;		
			var inflater = (LayoutInflater)_context.GetSystemService (Context.LayoutInflaterService);
			view = inflater.Inflate (Resource.Layout.textTypeface_row, null);
			var itemView = view.FindViewById<TextView> (Resource.Id.itemText);
			itemView.Text = item.TypefaceName;
			itemView.SetTypeface (item.Typeface, TypefaceStyle.Normal);
			return view;
		}


	}

}


