using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace OnlineWebApp.Models
{
    public partial class Business_Logics
    {
        ApplicationDbContext db = new ApplicationDbContext();

       string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        public static Business_Logics GetCart(HttpContextBase context)
        {
            var cart = new Business_Logics();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls 
        public static Business_Logics GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        } 

        public void AddToCart(Items items)
        {             // Get the matching cart and item instances  
            var cartItem = db.Carts.SingleOrDefault( c => c.CartId == ShoppingCartId  && c.Item_Id == items.Item_Id); 

            if (cartItem == null)
            {                 // Create a new cart item if no cart item exists  
                cartItem = new Cart
                {
                    Item_Id= items.Item_Id,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = System.DateTime.Now
                }; 

                db.Carts.Add(cartItem);
            }
            else
            {  // If the item does exist in the cart, then add one to the quantity 
                cartItem.Count++;
            }

            // Save changes
            db.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {             // Get the cart  
            var cartItem = db.Carts.Single( cart => cart.CartId == ShoppingCartId  && cart.RecordId == id); 
            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                { cartItem.Count--; itemCount = cartItem.Count; }
                else
                { db.Carts.Remove(cartItem); }
                // Save changes  
                db.SaveChanges();
            } 
                return itemCount;
            }

            public void EmptyCart()
            {
                var cartItems = db.Carts.Where(cart => cart.CartId == ShoppingCartId);

                foreach (var cartItem in cartItems) { db.Carts.Remove(cartItem); }

            // Save changes 
            db.SaveChanges();
        } 

    public List<Cart> GetCartItems()
    {
            return db.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();

    }  

    public int GetCount()
    {             // Get the count of each item in the cart and sum them up  
        int? count = (from cartItems in db.Carts
                      where cartItems.CartId == ShoppingCartId
                      select (int?)cartItems.Count).Sum();

        // Return 0 if all entries are null
        return count ?? 0;
    }

        public decimal GetTotal()
        {             
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total 

            decimal? total = (from cartItems in db.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Items.ItemCost).Sum();
            return total ?? decimal.Zero;
        }


        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();

            // Iterate over the items in the cart, adding the order details for each 
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetails
                {
                    Item_Id = item.Item_Id,
                    Order_Id = order.Order_Id,

                    UnitPrice = item.Items.ItemCost,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart 
                orderTotal += (item.Count * item.Items.ItemCost); 
            db.OrderDetails.Add(orderDetail);
            }
            // Set the order's total to the orderTotal count 
            order.Total= orderTotal;
            // Save the order
            db.SaveChanges();
            // Empty the shopping cart  
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.Order_Id;
        }


        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                { 
                    // Generate a new random GUID using System.Guid class 
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            } 
            return context.Session[CartSessionKey].ToString();
    }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username 
        public void MigrateCart(string userName)
        {
            var shoppingCart = db.Carts.Where(c => c.CartId == ShoppingCartId); 
 
            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
                db.SaveChanges();
        }
    } 
}
