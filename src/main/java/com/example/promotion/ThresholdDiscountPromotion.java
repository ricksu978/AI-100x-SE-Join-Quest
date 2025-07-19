package com.example.promotion;

import com.example.model.Order;

public class ThresholdDiscountPromotion {
    private int threshold;
    private int discount;

    public ThresholdDiscountPromotion(int threshold, int discount) {
        this.threshold = threshold;
        this.discount = discount;
    }

    public int calculateDiscount(Order order) {
        int originalAmount = 0;
        for (com.example.model.OrderItem item : order.getItems()) {
            originalAmount += item.getSubtotal();
        }

        if (originalAmount >= threshold) {
            return discount;
        }
        return 0;
    }

    public int getThreshold() {
        return threshold;
    }

    public int getDiscount() {
        return discount;
    }
}
