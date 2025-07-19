package com.example.service;

import com.example.model.Order;
import com.example.model.OrderItem;
import com.example.promotion.ThresholdDiscountPromotion;
import com.example.promotion.BuyOneGetOnePromotion;
import java.util.List;
import java.util.ArrayList;

public class OrderService {
    private List<ThresholdDiscountPromotion> promotions;
    private List<BuyOneGetOnePromotion> buyOneGetOnePromotions;

    public OrderService() {
        this.promotions = new ArrayList<>();
        this.buyOneGetOnePromotions = new ArrayList<>();
    }

    public void addPromotion(ThresholdDiscountPromotion promotion) {
        promotions.add(promotion);
    }

    public void addBuyOneGetOnePromotion(BuyOneGetOnePromotion promotion) {
        buyOneGetOnePromotions.add(promotion);
    }

    public Order processOrder(Order order) {
        // Calculate original amount
        int originalAmount = calculateOriginalAmount(order);

        // Apply buy-one-get-one promotions (affects quantity)
        applyBuyOneGetOnePromotions(order);

        // Apply discount promotions
        int discount = calculateDiscount(order);

        // Set final amounts
        order.setOriginalAmount(originalAmount);
        order.setDiscount(discount);
        order.setTotalAmount(originalAmount - discount);

        return order;
    }

    private int calculateOriginalAmount(Order order) {
        int total = 0;
        for (OrderItem item : order.getItems()) {
            total += item.getSubtotal();
        }
        return total;
    }

    private void applyBuyOneGetOnePromotions(Order order) {
        for (BuyOneGetOnePromotion promotion : buyOneGetOnePromotions) {
            promotion.applyPromotion(order);
        }
    }

    private int calculateDiscount(Order order) {
        int totalDiscount = 0;
        for (ThresholdDiscountPromotion promotion : promotions) {
            totalDiscount += promotion.calculateDiscount(order);
        }
        return totalDiscount;
    }
}
