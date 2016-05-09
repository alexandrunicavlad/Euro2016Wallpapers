using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Graphics;

namespace Euro2016Wallpapers
{
	
	public class TextListAdapter:BaseAdapter<string>
	{
		private List<String> _items;
		private Context _context;
		private List<Typeface> _typefaces;

		public TextListAdapter (Context context, List<String> items)
		{
			_context = context;
			_items = items;

		}

		public override int Count {
			get { return _items.Count; }
		}

		public override string this [int position] {
			get { return _items [position]; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = _items [position];
			var view = convertView;

			var inflater = (LayoutInflater)_context.GetSystemService (Context.LayoutInflaterService);
			view = inflater.Inflate (Resource.Layout.textItems_row, null);
			var itemView = view.FindViewById<TextView> (Resource.Id.itemText);
			itemView.Text = item;

			return view;
		}


	}

}

