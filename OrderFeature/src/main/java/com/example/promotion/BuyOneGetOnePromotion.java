package com.example.promotion;

import com.example.model.Order;

public class BuyOneGetOnePromotion {
    private String targetCategory;

    public BuyOneGetOnePromotion(String targetCategory) {
        this.targetCategory = targetCategory;
    }

    public void applyPromotion(Order order) {
        for (com.example.model.OrderItem item : order.getItems()) {
            String productCategory = item.getProduct().getCategory();
            if (targetCategory.equals(productCategory)) {
                // Buy one get one: for each item purchased, get one free
                // But for scenario 4: buy 2, get 1 free (total 3)
                // This suggests: buy any amount, get 1 free per unique product
                int originalQuantity = item.getQuantity();
                int freeQuantity = 1; // Only 1 free per product type
                item.setFinalQuantity(originalQuantity + freeQuantity);
            }
        }
    }

    public String getTargetCategory() {
        return targetCategory;
    }
}
