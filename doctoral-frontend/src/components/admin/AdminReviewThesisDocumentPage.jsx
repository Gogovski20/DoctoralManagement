import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { studentService } from "../../api/studentService";

export default function AdminReviewThesisDocumentPage() {
  const { projectId, documentId } = useParams();
  const navigate = useNavigate();

  const [newStatus, setNewStatus] = useState("Approved");
  const [reviewComment, setReviewComment] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const submitReview = async () => {
    setLoading(true);
    setError(null);

    try {
      await studentService.reviewThesisDocument(documentId, {
        newStatus,
        reviewComment: reviewComment || null
      });

      navigate(`/admin/doctoral-projects/${projectId}`);
    } catch (err) {
      console.error(err);
      setError(
        err.response?.data?.message || "Failed to review thesis document"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: "100vh", background: "#f9fafb", padding: "2rem" }}>
      <div
        style={{
          maxWidth: "700px",
          margin: "0 auto",
          background: "white",
          padding: "2rem",
          borderRadius: "0.75rem",
          border: "1px solid #e5e7eb"
        }}
      >
        <h1 style={{ fontSize: "1.75rem", fontWeight: "bold" }}>
          Review Thesis Document
        </h1>

        <p style={{ color: "#6b7280", marginBottom: "1.5rem" }}>
          Approve or reject the submitted thesis document.
        </p>

        {/* STATUS */}
        <div style={{ marginBottom: "1rem" }}>
          <label style={{ fontWeight: "600" }}>Decision</label>
          <select
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value)}
            style={{
              width: "100%",
              padding: "0.6rem",
              marginTop: "0.4rem",
              borderRadius: "0.4rem",
              border: "1px solid #d1d5db"
            }}
          >
            <option value="Approved">Approve</option>
            <option value="Rejected">Reject</option>
          </select>
        </div>

        {/* COMMENT */}
        <div style={{ marginBottom: "1.5rem" }}>
          <label style={{ fontWeight: "600" }}>Review Comment (optional)</label>
          <textarea
            rows={4}
            value={reviewComment}
            onChange={(e) => setReviewComment(e.target.value)}
            placeholder="Explain your decision..."
            style={{
              width: "100%",
              padding: "0.6rem",
              marginTop: "0.4rem",
              borderRadius: "0.4rem",
              border: "1px solid #d1d5db"
            }}
          />
        </div>

        {error && (
          <p style={{ color: "#dc2626", marginBottom: "1rem" }}>{error}</p>
        )}

        {/* ACTIONS */}
        <div style={{ display: "flex", gap: "1rem" }}>
          <button
            onClick={submitReview}
            disabled={loading}
            style={{
              background: newStatus === "Approved" ? "#16a34a" : "#dc2626",
              color: "white",
              padding: "0.6rem 1.4rem",
              borderRadius: "0.5rem",
              border: "none",
              cursor: "pointer"
            }}
          >
            {loading ? "Submitting..." : "Submit Review"}
          </button>

          <button
            onClick={() =>
              navigate(`/admin/doctoral-projects/${projectId}`)
            }
            style={{
              background: "#6b7280",
              color: "white",
              padding: "0.6rem 1.4rem",
              borderRadius: "0.5rem",
              border: "none",
              cursor: "pointer"
            }}
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
