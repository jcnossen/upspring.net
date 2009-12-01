using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ce.engine;

namespace UpspringSharp
{
	/*
	Collects a set of small textures and creates a big one out of it
	used for lightmap packing
	*/
	public class TextureBinTree
	{
		class Node
		{
			public Node () { }
			public Node (int X, int Y, int W, int H) {
				x = X; y = Y; w = W; h = H;
			}

			public int x,y,w,h;
			public int img_w, img_h;
			public Node[] child = new Node[2];
		};

		Node AddNode (Image subtex);
		
		void Init (int w, int h, ImgFormat fmt);
		bool IsEmpty () { return tree == null; }

		float GetU (Node n, float u) { return u * (float)n.img_w / (float)(texture.Width) + (n.x + 0.5f) / texture.Width; }
		float GetV (Node n, float v) { return v * (float)n.img_h / (float)(texture.Height) + (n.y + 0.5f) / texture.Height; }

		// Unused by class: uint for storing texture rendering id
		uint render_id;

		Image GetResult() { return texture; }

		void StoreNode (Node pm, Image tex);
		Node InsertNode (Node n, int w, int h);

		Node tree;
		Image texture;

		void Init (int w,int h, int format)
		{
			texture = new Image(w,h,3,format);
		}
		/*
		void StoreNode (Node n, Image tex)
		{
			n.img_w = tex.Width;
			n.img_h = tex.Height;

			tex->Blit (&texture, 0, 0, n->x, n->y, tex->w, tex->h);
		}

		TextureBinTree::Node* TextureBinTree::InsertNode (Node* n, int w, int h)
		{
			if (n->child [0] || n->child [1]) // not a leaf node ?
			{
				Node *r = 0;
				if (n->child [0]) r = InsertNode (n->child [0], w, h);
				if (r) return r;
				if (n->child [1]) r = InsertNode (n->child [1], w, h);
				return r;
			}
			else
			{
				// Occupied
				if (n->img_w)
					return 0;

				// Does it fit ?
				if (n->w < w || n->h < h)
					return 0;

				if (n->w == w && n->h == h)
					return n;

				int ow = n->w - w, oh = n->h - h;
				if (ow > oh)
				{
					// Split vertically
					if (ow) n->child [0] = new Node (n->x + w, n->y, ow, n->h);
					if (oh) n->child [1] = new Node (n->x, n->y + h, w, oh);
				}
				else
				{
					// Split horizontally
					if (ow) n->child [0] = new Node (n->x + w, n->y, ow, h);
					if (oh) n->child [1] = new Node (n->x, n->y + h, n->w, oh);
				}

				return n;
			}

			return 0;
		}

		TextureBinTree::Node* TextureBinTree::AddNode (Image *subtex)
		{
			Node *pn;

			if (!tree)
			{
				// create root node
				if (subtex->w > texture.w || subtex->h > texture.h)
					return 0;

				tree = new Node (0,0, texture.w, texture.h);
			}

			pn = InsertNode (tree, subtex->w, subtex->h);
			if (!pn) return 0;
			
			StoreNode (pn, subtex);
			return pn;
		}

		*/
	}
}
